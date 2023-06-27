using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Serialization.SystemTextJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace am
{    

/*======================================================================================================*/

public partial class LambdaFuncGetEnv : LambdaBaseArg
{    
    /*==[ env params  ]=====================================================================*/
    
    public string s3Bucket     { get; set; } = "";
    public string s3DirObject  { get; set; } = "LambdaFuncGetData/";
}

/*======================================================================================================*/

public class LambdaFuncGetResponse
{
    public string status { get; set; } = "";
    public string result { get; set; } = "";
}
    
/*======================================================================================================*/

public class LambdaFuncGet : LambdaGetBase {

    ILambdaContext? m_ctx;
    APIGatewayHttpApiV2ProxyRequest? m_data;
    
    public override string Handler(APIGatewayHttpApiV2ProxyRequest data, ILambdaContext context){
	
        m_ctx  = context;
        m_data = data;

	m_ctx.Log("Start Process");

        var result = Job(data, context);
	var resp   = JsonSerializer.Serialize(result.Result);	

	m_ctx.Log("Complete Process ( Wait Async Method )");
	
        return resp;
    }
    
    public static async Task<LambdaFuncGetResponse> Job(APIGatewayHttpApiV2ProxyRequest data, ILambdaContext ctx){
        
        var ret = await Task.Run(() => {

	    var resp = new LambdaFuncGetResponse{
		status = ApiError.E_CHAOS.ToString(),
		result = ""
	    };
	    
	    try {
		resp.status = ApiError.E_OK.ToString();
	    }
	    catch(Exception e){
		ctx.Log(e.ToString());
		resp.status = ApiError.E_CRITICAL.ToString();
	    }
                
	    return resp;
	});
        
        return ret;         
    }
    
}

/*======================================================================================================*/
}
