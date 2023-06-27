#######################################################################################

.SUFFIXES : .php .json

#######################################################################################

include Makefile.env

# 上記で設定されるパラメータ、API_ はテスト実行時のルールで用いるパラメータ
PROJECT      ?= 
API_FUNC     ?=
API_FUNC_ARG ?=
API_STAGE    ?=
API_NAME     ?= $(shell cat serverless.yml | yq '.functions.${API_FUNC}.events[0].http.path' | tr -d "\"")
API_URL      ?= $(shell cat ApiInfo-${API_STAGE}.json | jq '.list[] | select (.url | endswith("${API_NAME}")) | .url '  | tr -d "\"")
API_KEY      ?= ${shell cat ApiInfo-${API_STAGE}.json | jq '.key' | tr -d "\""}

# LocalLambda実行時の環境変数設定（make run でのテスト時に参照させる環境変数）
LAMBDA_ENV_VAR  = export LAMBDA_SERVICE=${PROJECT}; 
LAMBDA_ENV_VAR += export LAMBDA_ENV=${API_STAGE}; 
LAMBDA_ENV_VAR += export DEBUG_API_URL=${API_URL}; 
LAMBDA_ENV_VAR += export DEBUG_API_KEY=${API_KEY}; 

#######################################################################################

default:

env:
	@echo "PROJECT       ?= ${PROJECT}"       > Makefile.env
	@echo "API_FUNC      ?= ${API_FUNC}"     >> Makefile.env
	@echo "API_FUNC_ARG  ?= ${API_FUNC_ARG}" >> Makefile.env
	@echo "API_STAGE     ?= ${API_STAGE}"    >> Makefile.env

Makefile.env: env

##[ Lambda Build ]####################################################################

all: init build run

build:
	dotnet build --configuration Release

new:
	dotnet new console

init:
	dotnet restore

run:
	${LAMBDA_ENV_VAR} dotnet run

publish:
	dotnet publish --output arc

clear:
	rm -rf arc/*

clean:
	rm -rf arc
	rm -rf obj
	rm -rf bin

package: publish
	(cd arc && zip ../publish.zip *)

add-library:
	dotnet add package ${PKG}

lambda: publish package

##[ Serverless Framework ]#############################################################

serverlessConfig.yml:
	echo "service: ${PROJECT}" > serverlessConfig.yml

api-all: serverlessConfig.yml
#	serverless deploy --verbose --stage ${API_STAGE}
	serverless deploy --stage ${API_STAGE}

api: serverlessConfig.yml
	serverless deploy function -f ${API_FUNC} --stage ${API_STAGE}

# Terraformと違って、書き換えたら勝手に消すとかしてくれない
api-remove:
	serverless remove --verbose --stage ${API_STAGE}

# Lambda 直接呼のテスト
api-test:
	serverless invoke --function ${API_FUNC} -p ${API_FUNC_ARG} --log --stage ${API_STAGE}

# LambdaをAPIGatway経由で呼ぶ場合（APIキーなし）
api-test-gw:
	curl -X POST -H 'Content-Type:application/json' -d @${API_FUNC_ARG} ${API_URL}

# LambdaをAPIGatway経由で呼ぶ場合（APIキーあり）
api-test-gwk:
	curl -X POST -H 'Content-Type:application/json' -d @${API_FUNC_ARG} ${API_URL} --header x-api-key:${API_KEY}

# あとから確認したい用
api-log:
	serverless logs --function ${API_FUNC} --stage ${API_STAGE}

export-api-list:
	serverless info --stage ${API_STAGE} | grep "https://" | awk '{print $$4}' > apiList.txt

export-api-key:
	serverless info --stage ${API_STAGE} | grep -a1 "api keys" | tail -n1 | cut -d":" -f2 | awk '{print $$1}' > apiKey.txt

api-info: export-api-list export-api-key
	php tools/CreateApiInfo.php > ApiInfo-${API_STAGE}.json
	cat ApiInfo-${API_STAGE}.json | jq .
#	aws s3 cp ApiInfo-${API_STAGE}.json s3://${S3_BUCKET}/InfoData/ApiInfo-${API_STAGE}.json

