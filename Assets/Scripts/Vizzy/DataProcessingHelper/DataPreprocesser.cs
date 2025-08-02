using Newtonsoft.Json;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Craft;
using ModApi.Craft.Program.Instructions;
using ModApi.Craft.Program.Expressions;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Assets.Scripts.Vizzy.DataProcessingHelper
{
    [Serializable]
    public class NamedDataInJsonExpression : ProgramExpression
    //从JSON格式的文本信息中提取信息
    {
        public static string XmlName = "NamedDataInJson";
        public override bool IsBoolean { get => false; }
        public override ExpressionResult Evaluate(IThreadContext context)
        {
            ExpressionResult result = new();//取一半成品结果暂放一旁
            string pairName = this.GetExpression(0).Evaluate(context).TextValue;
            string rawJsonData = this.GetExpression(1).Evaluate(context).TextValue;
            var ParsedContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawJsonData);
            var target = ParsedContent[pairName];
            Debug.Log($"Helper: extracted recieved data to '{target.ToString()}'.");
            result.TextValue = target.ToString();//测试过，报错了会直接中止程序块运行😁

            //if (target is string){result.TextValue = (string)target;}
            //else if (target is bool){result.BoolValue = (bool)target;
            //else if (target is long or BigInteger or double){result.Nu
            //else if (target is JObject){ result.TextValue = target.ToString(Formatting.None); }
            return result;
        }
    }
    [Serializable]
    public class SeparateString2List : ProgramExpression
    //将文本按分隔符分离并存进List（Vizzy++的不知怎么的不能用）
    {
        public static string XmlName = "SeparateString-DPH";
        public override bool IsBoolean { get => false; }
        public override ExpressionResult Evaluate(IThreadContext context)
        {
            string data = this.GetExpression(0).Evaluate(context).TextValue;
            string separator = this.GetExpression(1).Evaluate(context).TextValue;
            string[] rawList = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            List<ExpressionListItem> resultList = new();

            for (int x = 0; x < rawList.Length; x++)
            {
                resultList.Add(rawList[x]);
            }

            ExpressionResult result = new(resultList);
            return result;
        }
    }
}
