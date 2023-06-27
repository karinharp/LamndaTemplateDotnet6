using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Serialization.SystemTextJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;

// 独自引数にAPIGatewayのマッピングとかをソースジェネレーター経由で出来ると公式が言ってるが、できないので断念
// https://docs.aws.amazon.com/ja_jp/lambda/latest/dg/csharp-handler.html
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
//[assembly: LambdaSerializer(typeof(SourceGeneratorLambdaJsonSerializer<am.LambdaArg>))]
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace am
{

public abstract class LambdaBaseArg
{
    /*==[ lambda args ]=====================================================================*/
    
    public string evt          { get; set; } = "default"; // トリガー判定用

    // header
    public string mode         { get; set; } = ""; // APIGatewayで強制上書きしてもいいかも
    
    // body
    public string dataBody     { get; set; } = ""; // base64 encoded Json（汎用なので、独自の構造体の方をつかうのも）
    
    /*======================================================================================*/

    //dotnet6だとv2のAWSSDKが使えないので、この書き方は無理
    //public RegionEndpoint region { get { return Amazon.RegionEndpoint.APNortheast1; }}
   
    /*======================================================================================*/
    
    public uint timestamp {
	get {
	    var timespan = m_now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	    return (uint)timespan.TotalSeconds;
	}
    }
    public Stopwatch sw { get { return m_sw; }}
    
    /*======================================================================================*/
    
    protected DateTime  m_now; 
    protected Stopwatch m_sw;
    
    public LambdaBaseArg(){
	m_now = DateTime.UtcNow;
	m_sw  = new Stopwatch();	
    }
}

/*======================================================================================*/
    
public abstract class LambdaPostBase<T_ARG> where T_ARG : LambdaBaseArg {
    public virtual string Handler(T_ARG data, ILambdaContext context){ return ""; }        
}

public abstract class LambdaGetBase {
    public virtual string Handler(APIGatewayHttpApiV2ProxyRequest data, ILambdaContext context){ return ""; }        
}

/*======================================================================================*/    
}
