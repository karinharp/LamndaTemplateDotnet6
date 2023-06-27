using System;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

namespace am {
    
    class Program {
	
        static void Main(string[] args){

	    Console.WriteLine("======================================================");
	    
	    TestRun(new LambdaFuncGet(), new APIGatewayHttpApiV2ProxyRequest(){});	    
	    TestRun(new LambdaFuncPost(), new LambdaFuncPostArg(){});
	    
        }

	static void TestRun<T_FUNC, T_ARG>(T_FUNC lambdaFunc, T_ARG lambdaArg)
	    where T_FUNC: LambdaPostBase<T_ARG>
	    where T_ARG: LambdaBaseArg
	{
	    Console.WriteLine(lambdaFunc.ToString() + " >> " + lambdaArg.mode + " >> Test.");
	    var ret = lambdaFunc.Handler(lambdaArg, new TestLambdaContext());
	    Console.WriteLine("Ret > " +  ret);
	    Console.WriteLine("======================================================");
	}

	static void TestRun<T_FUNC>(T_FUNC lambdaFunc,  APIGatewayHttpApiV2ProxyRequest req)
	    where T_FUNC: LambdaGetBase
	{
	    Console.WriteLine(lambdaFunc.ToString()+ " >> Test.");
	    var ret = lambdaFunc.Handler(req, new TestLambdaContext());
	    Console.WriteLine("Ret > " +  ret);
	    Console.WriteLine("======================================================");
	}
	
	static void _TestRun<T_FUNC, T_ARG>(T_FUNC lambdaFunc, T_ARG lambdaArg)
	    where T_FUNC: LambdaPostBase<T_ARG>
	    where T_ARG: LambdaBaseArg
	{
	}

	static void _TestRun<T_FUNC>(T_FUNC lambdaFunc,  APIGatewayHttpApiV2ProxyRequest req)
	    where T_FUNC: LambdaGetBase
	{
	}
	
    }
}
