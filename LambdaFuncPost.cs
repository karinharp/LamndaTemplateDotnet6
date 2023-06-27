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

public partial class LambdaFuncPostArg : LambdaBaseArg
{
    /*==[ lambda args ]=====================================================================*/

    public string postData     { get; set; } = "";
    
    /*==[ env params  ]=====================================================================*/
    
    public string s3Bucket     { get; set; } = "";
    public string s3DirObject  { get; set; } = "LambdaFuncPostData/";
}

/*======================================================================================================*/

public class LambdaFuncPostResponse
{
    public string status { get; set; } = "";	
    public string result { get; set; } = "";
}

/*======================================================================================================*/

public class LambdaFuncPost : LambdaPostBase<LambdaFuncPostArg> {

    ILambdaContext?         m_ctx;
    LambdaFuncPostArg?      m_data;
    
    public override string Handler(LambdaFuncPostArg data, ILambdaContext context){
	
        m_ctx  = context;
        m_data = data;

	m_data.sw.Start();
		
	m_ctx.Log(String.Format("{0:D8} : Start Process", m_data.sw.ElapsedMilliseconds));

        var result = Job(data, context);
	var resp   = JsonSerializer.Serialize(result.Result);	

	m_ctx.Log(String.Format("{0:D8} : Complete Process ( Wait Async Method )", m_data.sw.ElapsedMilliseconds));
	
        return resp;
    }
    
    public static async Task<LambdaFuncPostResponse> Job(LambdaFuncPostArg data, ILambdaContext ctx){
	
        var ret = await Task.Run(() => {
	    
	    var resp = new LambdaFuncPostResponse {
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
