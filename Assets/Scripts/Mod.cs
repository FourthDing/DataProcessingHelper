//using HarmonyLib;// Juno Harmony已经提供Harmony了（我就硬写）（已解决，装了个Harmony，不知道是不是Thin）
// 没有Harmony，编辑器疯狂报错，我都害怕（bushi
using ModApi.Craft.Program;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using System.Reflection;
using System.Text;
using Assets.Scripts.Vizzy.UI;
using ModApi;
using ModApi.Common;
using ModApi.Mods;
using ModApi.Ui;
using ModApi.Ui.Events;

using Assets.Scripts.Vizzy.DataProcessingHelper;
namespace Assets.Scripts
{
    

    /// <summary>
    /// A singleton object representing this mod that is instantiated and initialize when the mod is loaded.
    /// </summary>
    public class Mod : ModApi.Mods.GameMod
    {
        private static readonly IDictionary<string, (Type, Func<ProgramNode>)> AppendedVizzyBlocks;
        //抄了VizzyGL.
        private static FieldInfo VizzyToolboxColorsField;
        private static FieldInfo VizzyToolboxStylesField;
        private static VizzyToolbox _helperToolbox;
        private static VizzyToolbox HelperToolbox
        {
            get
            {
                if (_helperToolbox == null)
                {
                    var _helperToolboxXml =
                        ModApi.Common.Game.Instance.UserInterface.ResourceDatabase.GetResource<TextAsset>("Data Processing Helper/Vizzy/HelperToolbox");
                    if (_helperToolboxXml != null)
                    {
                        _helperToolbox =
                            new VizzyToolbox(XElement.Parse(_helperToolboxXml.text), false);
                    }
                    else
                    {
                        Debug.LogError("Helper Oops: The HelperToolbox Resource Was Not Found.");
                    }
                }
                return _helperToolbox;
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="Mod"/> class from being created.
        /// </summary>
        static Mod()
        {
            //Harmony harmony = new Harmony("Vizzy blocks to help process datas from extrenal source.");
            //harmony.PatchAll();
            //RegisterVizzyBlocks();
            AppendedVizzyBlocks = new Dictionary<string, (Type, Func<ProgramNode>)>()
            {
                [NamedDataInJsonExpression.XmlName] = (typeof(NamedDataInJsonExpression), () => new NamedDataInJsonExpression()),
                [SeparateString2List.XmlName] = (typeof(SeparateString2List), () => new SeparateString2List())
            };
            VizzyToolboxColorsField =
                typeof(VizzyToolbox).GetField("_colors", BindingFlags.NonPublic | BindingFlags.Instance);
            VizzyToolboxStylesField =
                typeof(VizzyToolbox).GetField("_styles", BindingFlags.NonPublic | BindingFlags.Instance);

            var programNodeCreatorType =//试图获取ProgramNodeCreator类型
                typeof(ProgramSerializer).GetNestedType("ProgramNodeCreator", BindingFlags.NonPublic);
            var programSerializerTypeNameLookupField =//获取类型字段
                typeof(ProgramSerializer).GetField("_typeNameLookup", BindingFlags.NonPublic | BindingFlags.Static);
            var programSerializerXmlNameLookupField =//获取XML表示名称字段（也许和类型字段一一对应？）
                typeof(ProgramSerializer).GetField("_xmlNameLookup", BindingFlags.NonPublic | BindingFlags.Static);

            if (programNodeCreatorType != null &&
                programSerializerTypeNameLookupField != null &&
                programSerializerXmlNameLookupField != null)
            {//检测获取是否成功
                var programNodeCreatorConstructor =//获取工厂（构建器）
                    programNodeCreatorType.GetConstructor(new[] {
                        typeof(String),
                        typeof(Type),
                        typeof(Func<ProgramNode>)
                    });

                if (programNodeCreatorConstructor != null)//检测获取工厂是否成功
                {
                    var typeNameLookup = (IDictionary)programSerializerTypeNameLookupField.GetValue(null);
                    var xmlNameLookup = (IDictionary)programSerializerXmlNameLookupField.GetValue(null);

                    foreach (var kvp in AppendedVizzyBlocks)
                    {
                        var xmlName = kvp.Key;
                        var (type, ctor) = kvp.Value;
                        Debug.Log($"Helper: Registering Block {xmlName}");
                        var programNodeCreator =//创建一个ProgramNodeCreator（看来程序块就是从这里被拖出来的）
                            programNodeCreatorConstructor.Invoke(//让工厂做一个程序块生成器
                                new System.Object[] {
                                    xmlName,
                                    type,
                                    ctor
                                });
                        xmlNameLookup[xmlName] = typeNameLookup[type.Name] = programNodeCreator;//向两个表注册生成器
                    }
                    Debug.Log($"Helper: Block registering completed with no errors.");
                }
                else
                {
                    Debug.Log("Helper Oops: Constructor for ProgramNodeCreator not found.");
                }
            }
            else
            {
                Debug.LogError(
                    "Helper Oops: Reflection Failed. Unable to find expected internal type ProgramSerializer.ProgramNodeCreator, or one of the expected private fields _typeNameLookup or _xmlNameLookup on ProgramSerializer.");
            }
        }
        private Mod() : base() { }
        protected override void OnModInitialized()
        {
            ModApi.Common.Game.Instance.UserInterface.UserInterfaceLoading += UiOnUserInterfaceLoading;

            base.OnModInitialized();

            // DataProcessingHelperVizzyBlocks.Initialize();
        }

        private void UiOnUserInterfaceLoading(object sender, UserInterfaceLoadingEventArgs e)
        {
            var vizzyUI = e.XmlLayout.GameObject.GetComponent<VizzyUIController>();//c
            if (e.UserInterfaceId == UserInterfaceIds.Vizzy)
            {
                if (HelperToolbox != null)
                {
                    if (vizzyUI.VizzyUI.Toolbox != null)
                    {
                        MergeToolbox(//合并Toolbox
                            vizzyUI.VizzyUI.Toolbox,
                            HelperToolbox
                        );
                    }
                    else
                    {
                        Debug.Log("Helper Emm: The default Vizzy Toolbox isn't loaded yet.");
                    }
                }
                else
                {
                    Debug.Log("Helper Emm: Unable to load HelperToolbox.");
                }
            }
        }

        /// <summary>
        /// Gets the singleton instance of the mod object.
        /// </summary>
        /// <value>The singleton instance of the mod object.</value>
        public static Mod Instance { get; } = GetModInstance<Mod>();
        //疑惑：为什么要注释符后面要加一个空格再写

        //private static readonly Dictionary<string, (Type, Func<ProgramNode>)> AppendedVizzyBlocks = new()
        //{
        //    ["NamedDataInJson"] = (typeof(NamedDataInJsonExpression), () => new NamedDataInJsonExpression()),
        //};
        // 结构上抄了Sockets Service，此Mod也几乎是为辅助该Mod而生
        //public static void RegisterVizzyBlocks()//名字解释了用途
        //{
        //直接抄Sockets Service了😅

        // 获取 ProgramNodeCreator 类型，使用 AccessTools.Inner 来获取嵌套类型
        //    var programNodeCreatorType = /*Harmony的*/AccessTools.Inner(typeof(ProgramSerializer), "ProgramNodeCreator");
        //    if (programNodeCreatorType == null)
        //    {
        //        Debug.LogError("Registration failed: Could not find ProgramNodeCreator type");
        //        return;
        //    }

        //    var creatorConstructor = programNodeCreatorType.GetConstructor(new[] { typeof(string), typeof(Type), typeof(Func<ProgramNode>) });
        //    if (creatorConstructor == null)
        //    {
        //        Debug.LogError("Registration failed: Could not find ProgramNodeCreator constructor");
        //        return;
        //    }

        // 获取序列化器内部字典
        //    var typeNameLookup = AccessTools.Field(typeof(ProgramSerializer), "_typeNameLookup")?.GetValue(null) as IDictionary;
        //    var xmlNameLookup = AccessTools.Field(typeof(ProgramSerializer), "_xmlNameLookup")?.GetValue(null) as IDictionary;

        //    if (typeNameLookup == null || xmlNameLookup == null)
        //    {
        //        Debug.LogError("Registration failed: Could not get internal dictionaries of the serializer");
        //        return;
        //    }
        //    foreach (var (xmlName, (nodeType, constructor)) in AppendedVizzyBlocks)
        //    {
        //        var creator = creatorConstructor.Invoke(new object[] { xmlName, nodeType, constructor });
        //        typeNameLookup[nodeType.Name] = creator;
        //        xmlNameLookup[xmlName] = creator;

        //        Debug.Log($"Successfully registered custom node: {xmlName}");
        //    }
        //}
        //抄的Vizzy++，至少要看明白。
        private static void MergeToolbox(
            VizzyToolbox baseToolbox,
            VizzyToolbox extensionToolbox)
        {
            Debug.Log("Start merging toolbox.");
            //基本的有效性检查。
            if (VizzyToolboxColorsField == null)
            {
                throw new InvalidOperationException($"{nameof(VizzyToolboxColorsField)} is null.");
            }
            else if (VizzyToolboxStylesField == null)
            {
                throw new InvalidOperationException($"{nameof(VizzyToolboxStylesField)} is null.");
            }
            else
            {
                Debug.Log("Toolbox entry field ok.");

            }
            //原Toolbox颜色和代码块样式
            var baseColors = (Dictionary<String, Color>)VizzyToolboxColorsField.GetValue(baseToolbox);
            var baseStyles = (Dictionary<String, NodeStyle>)VizzyToolboxStylesField.GetValue(baseToolbox);

            //添加的代码块颜色和样式
            var extensionColors = (Dictionary<String, Color>)VizzyToolboxColorsField.GetValue(extensionToolbox);
            foreach (var color in extensionColors)
            {//合并颜色
                Debug.Log($"Helper: Adding new color {color.Key}");
                baseColors[color.Key] = color.Value;
            }
            var extensionStyles = (Dictionary<String, NodeStyle>)VizzyToolboxStylesField.GetValue(extensionToolbox);
            foreach (var style in extensionStyles)
            {//合并样式
                Debug.Log($"Helper: Adding new style {style.Key}");
                baseStyles[style.Key] = style.Value;
            }

            foreach (var category in extensionToolbox.Categories)
            {//合并类别表
                var existingCategory =
                    baseToolbox.Categories.SingleOrDefault(c => String.Equals(c.Name, category.Name, StringComparison.OrdinalIgnoreCase));
                if (existingCategory == null)
                {
                    baseToolbox.Categories.Add(category);//没有则添加
                }
                else
                {
                    foreach (var node in category.Nodes)
                    {//有则合并
                        if (!existingCategory.Nodes.Any(n => String.Equals(n.Style, node.Style)))
                        {//检测是否重复
                         // TODO: Make position configurable some how.
                            Debug.Log($"Helper: adding block '{node.ToString()}'");
                            existingCategory.Nodes.Add(node);
                        }
                        else
                        {
                            Debug.Log($"Helper Emm: Unable to add new vizzy node, duplicate style id: '{node.Style}'");
                        }
                    }
                }
            }
            Debug.Log("Toolbox Merged.");
        }
    }
}