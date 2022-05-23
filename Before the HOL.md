Protect and Scale Apps Hands-on lab  
June 2022

<br />

### 参考情報
- <a href="https://docs.microsoft.com/ja-jp/azure/cloud-adoption-framework/ready/azure-best-practices/resource-naming">名前付け規則を定義する</a>

- <a href="https://docs.microsoft.com/ja-jp/azure/cloud-adoption-framework/ready/azure-best-practices/resource-abbreviations">Azure リソースの種類に推奨される省略形</a>

<br />

## リソースの展開

<br />

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fkohei3110%2FPublic-Application-Management-Hands-on-Lab%2Fmaster%2Ftemplates%2Fdeploy-resources.json)

### パラメーター
- **virtualNetworkName**: 仮想ネットワーク名（長さ：2 ～ 64 / 有効な文字：英数字、アンダースコア、ピリオド、およびハイフン）

- **addressPrefix**: IPv4 アドレス空間

- **subnetName1**: サブネットの名前 (1)（長さ：1 ～ 80 / 有効な文字：英数字、アンダースコア、ピリオド、およびハイフン）

- **subnetPrefix1**: サブネット アドレス範囲 (1)

- **subnetName2**: サブネットの名前 (2)（長さ：1 ～ 80 / 有効な文字：英数字、アンダースコア、ピリオド、およびハイフン）

- **subnetPrefix2**: サブネット アドレス範囲 (2)

- **bastionHostName**: Azure Bastion の名前（長さ：1 ～ 80  / 有効な文字：英数字、アンダースコア、ピリオド、およびハイフン）

- **virtualMachineName**: 仮想マシンの名前（長さ：1 ～ 15（Windows）/ スペース、制御文字等は使用不可）

- **adminUserName**: 仮想マシンのローカル管理者（長さ：1 ～ 20 / 特殊文字 \/""[]:|<>+=;,?*@& の使用不可）

- **adminPassword**: 仮想マシンのローカル管理者のパスワード（長さ：12 ～ 123 / 大文字、小文字、数字、特殊文字のうち３つを含む）

- **webAppName**: Web アプリの名前（一意に識別できる名前 / 長さ：2 ～ 60  / 有効な文字：英数字とハイフン）

- **cosmosAccountName**: Cosmos DB アカウント（一意に識別できる名前 / 長さ：3 ～ 44 / 有効な文字：小文字、数字、およびハイフン）

<br />

※事前にリソース グループの作成が必要

※選択したリソース グループのリージョンにすべてのリソースが展開

<br />

## 環境設定

### Cosmos DB へのデータコピー
