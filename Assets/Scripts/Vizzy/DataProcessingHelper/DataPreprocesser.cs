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
            Debug.Log("Helper: Now create new result.");
            ExpressionResult result = new();//取一半成品结果暂放一旁
            Debug.Log("Helper: Now get value from two args.");
            string pairName = this.GetExpression(0).Evaluate(context).TextValue;
            Debug.Log($"Helper: got target name'{pairName}'.");
            string rawJsonData = this.GetExpression(1).Evaluate(context).TextValue;
            Debug.Log($"Helper: got target JSON'{rawJsonData}'.");
            Debug.Log("Helper: Now parse raw JSON string.");
            var ParsedContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawJsonData);
            Debug.Log("Helper: Completed JSON string parseing.");
            var target = ParsedContent[pairName];
            result.TextValue = target.ToString();//测试过，报错了会直接中止程序块运行😁
            //if (target is string){result.TextValue = (string)target;}
            //else if (target is bool){result.BoolValue = (bool)target;}
            //else if (target is long or BigInteger or double){result.NumberValue = (double)target;}
            //else if (target is JObject){ result.TextValue = target.ToString(Formatting.None); }
            return result;
        }
    }
}
