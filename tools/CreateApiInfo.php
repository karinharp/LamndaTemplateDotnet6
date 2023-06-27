<?php

$apiInfo = array();

$apiInfo["lastupdate"] = time();
$apiInfo["list"] = array();

$list = file("apiList.txt");

foreach($list as $url){
    $record = array();
    $tmp    = explode("/", trim($url));
    array_splice($tmp, 0, 4);
    $record["name"] = implode("/", $tmp);
    $record["url"]  = trim($url);
    $record["key"]  = "";
    array_push($apiInfo["list"], $record);
}

$record = array();

// 別系統の情報を埋め込む時はここにハードコード
/*
$record["name"] = "S3Proxy";
$record["url"]  = "https://";
$record["key"]  = "";
array_push($apiInfo["list"], $record);
*/

$apiInfo["key"]  = trim(file_get_contents("apiKey.txt"));

// 追加で必要なパラメータを定義
/*
$apiInfo["revision"]     = 23062301;
$apiInfo["isMaint"]      = 0;
*/

//echo json_encode($apiInfo, JSON_PRETTY_PRINT);
echo json_encode($apiInfo);

