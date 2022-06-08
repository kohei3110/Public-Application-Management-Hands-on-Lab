![Microsoft Cloud Workshop](images/ms-cloud-workshop.png)

Protect and Scale Apps Hands-on lab  
June 2022

<br />

### **Contents**

- [Advance Preparation: アプリケーションの動作確認](#advance-preparation-アプリケーションの動作確認)

- [Exercise 1: Azure リソースの保護](#exercise-1-azure-リソースの保護)

  - [Task 1: Microsoft Defender for Cloud の有効化](#task-1-microsoft-defender-for-cloud-の有効化)

  - [Task 2: 不正アクセスの検出](#task-2-不正アクセスの検出)

- [Exercise 2: Cosmos DB アカウントへのアクセス制限](#exercise-2-cosmos-db-アカウントへのアクセス制限)

  - [Task 1: Cosmos DB アカウントへの Private Link の構成](#task-1-cosmos-db-アカウントへの-private-link-の構成)

  - [Task 2: App Service 仮想ネットワーク統合](#task-2-app-service-仮想ネットワーク統合)

- [Exercise 3: アプリケーションの保護](#exercise-3-アプリケーションの保護)

  - [Task 1: Front Door の作成・構成](#task-1-front-door-の作成・構成)

  - [Task 2: ログの確認](#task-2-ログの確認)

- [Exercise 4: マルチリージョン展開による高可用性構成](#exercise-4-マルチリージョン展開による高可用性構成)

  - [Task 1: リソースの作成](#task-1-リソースの作成)

  - [Task 2: VNet Peering の構成](#task-2-vnet-peering-の構成)

  - [Task 3: Cosmos DB グローバル分散](#task-3-cosmos-db-グローバル分散)

  - [Task 4: Cosmos DB Private Endpoint の追加](#task-4-cosmos-db-private-endpoint-の追加)

  - [Task 5: App Service の作成・構成](#task-5-app-service-の構成)

  - [Task 6: Front Door 配信元グループへの App Service の追加](#task-6-front-door-配信元グループへの-app-service-の追加)

<br />

## 使用する環境

<img src="images/Azure-resources.png" />

<br />

## Advance Preparation: アプリケーションの動作確認

- App Service の **概要** ページの **URL** をクリック

- 新しいタブでアプリケーションが表示

  <img src="images/web-app-1.png" />

- Name 列の項目に含まれている文字列を検索ワードに指定し検索を実行

  <img src="images/web-app-2.png" />

  ※ 大文字、小文字を判別するため注意

- 検索ワードに以下を指定し、再度検索を実行

- JavaScript を埋め込み

  ```
  ' --<script>alert('1');</script>
  ```

  <img src="images/web-app-4.png" />

  <br />

- 他サイトの情報を埋め込み

  ```
  ' --<iframe width="100%" height="166" scrolling="no" frameborder="no" allow="autoplay" src="https://w.soundcloud.com/player/?url=https%3A//api.soundcloud.com/tracks/771984076&color=%23ff5500&auto_play=true&hide_related=false&show_comments=true&show_user=true&show_reposts=false&show_teaser=true"></iframe>
  ```

  <img src="images/web-app-3.png" />

<br />

※ XSS 攻撃が成功し、JavaScript の実行や他サイトの情報が表示されることを確認

<br />

## Exercise 1: Azure リソースの保護

<img src="images/exercise1.png" />

<br />

### Task 1: Microsoft Defender for Cloud の有効化

- Azure ポータルのトップ画面から **ツール** に表示される **Microsoft Defender for Cloud** をクリック

  <img src="images/enabled-defender-for-cloud-1.png" />

- **環境設定** ページを表示し、保護を行うサブスクリプションを選択

- **すべての Microsoft Defender for Cloud プランの有効化** を選択

- **App Service** と **データベース** の状態を **オン** に設定

  <img src="images/enabled-defender-for-cloud-3.png" />

- データベースのリソースの種類で **Cosmos DB** が **オン** に設定されていることを確認

    <img src="images/enabled-defender-for-cloud-4.png" />

- **保存** をクリック

  ※ 最初の 30 日間は無料でお使いいただけます

<br />

### Task 2: 不正アクセスの検出

- Cosmos DB の管理ブレードへ移動

- **データ エクスプローラー** を選択し、データベースを展開し **Products** コンテナーを選択

- **New SQL Query** をクリック

  <img src="images/cosmos-sql-injection-1.png" />

- 特定レコードを抽出するクエリを実行

  ```
  SELECT * FROM c 
  WHERE c.productId = 709
  ```

  <img src="images/cosmos-sql-injection-2.png" />

- WHERE 句に新たな条件を指定しクエリを実行

  ```
  SELECT * FROM c 
  WHERE c.productId = 709
  OR 1 = 1
  ```

  <img src="images/cosmos-sql-injection-3.png" />

  ※ 前の条件を無視し全レコードが抽出されることを確認

<br />

**※ セキュリティ アラートが表示されるまで時間がかかるので先の手順を進め、後からご確認ください**

- Cosmos DB の管理ブレードから **Security Center** を選択

- 通知されたアラートをクリック

  <img src="images/defender-for-cloud-alert-1.png" />

- セキュリティ アラートの画面で再度アラートをクリック

  <img src="images/defender-for-cloud-alert-2.png" />

- 画面右にアラートの説明が表示

- **すべての詳細を表示** をクリック

  <img src="images/defender-for-cloud-alert-3.png" />

- 対象のリソースや疑わしいクエリなどアラートの詳細を確認

  <img src="images/defender-for-cloud-alert-4.png" />

- **アクションの実行** タブを表示

- メールでの通知設定やアラートが通知された際の処理を指定できることを確認

  <img src="images/defender-for-cloud-alert-5.png" />

<br />

## Exercise 2: Cosmos DB アカウントへのアクセス制限

<img src="images/exercise2.png" />

<br />

### Task 1: Cosmos DB アカウントへの Private Link の構成

- Cosmos DB の管理ブレードから **プライベート エンドポイント接続** を選択

- **＋ プライベート エンドポイント** をクリック

  <img src="images/cosmos-private-endpoint-01.png" />

- **基本** タブでサブスクリプション、リソース、名前、地域を指定

  - **サブスクリプション**: ワークショップで使用しているサブスクリプション

  - **リソース グループ**: ワークショップで使用しているリソース グループ

  - **名前**: 任意（pl-cosmos-workshop-xxx など）

  - **Network Interface Name**: 任意（既定のままで OK）

  - **地域**: 仮想ネットワークが展開されている地域を選択

    <img src="images/cosmos-private-endpoint-02.png" />

- **リソース** タブでプライベート エンドポイントを作成するリソースを指定

  - **サブスクリプション**: ワークショップで使用しているサブスクリプション

  - **リソースの種類**: Microsoft.AzureCosmosDB/databaseAccounts

  - **リソース**: ワークショップで使用している Cosmos DB アカウント

  - **対象サブ リソース**: Sql

    <img src="images/cosmos-private-endpoint-03.png" />

- **仮想ネットワーク** タブでプライベート エンドポイントを作成する仮想ネットワーク、サブネットを選択

  <img src="images/cosmos-private-endpoint-04.png" />

- **DNS** タブでプライベート DNS ゾーンを設定

  - **プライベート DNS ゾーンと統合する**: はい

  - **プライベート DNS ゾーン**: 新規

    <img src="images/cosmos-private-endpoint-05.png" />

- **タグ** は既定のまま **次: 確認および作成 >** をクリック

  <img src="images/cosmos-private-endpoint-06.png" />

- 設定内容を確認し **作成** をクリック

  <img src="images/cosmos-private-endpoint-07.png" />

- 作成したプライベート エンドポイントをクリック

  <img src="images/cosmos-private-endpoint-09.png" />

- DNS の構成を確認

  <img src="images/cosmos-private-endpoint-10.png" />

  ※ グローバル エンドポイントと Cosmos DB アカウントが展開されているリージョン用の IP アドレス、FQDN が作成

- Cosmos DB の管理ブレードから **データ エクスプローラー** を選択

- データベースを展開し Products コンテナーの items を選択

  リクエストがブロックされ結果が表示されないことを確認

  <img src="images/cosmos-private-endpoint-08.png" />

<br />

### Task 2: App Service 仮想ネットワーク統合

- App Service の管理ブレードから **ネットワーク** を選択

- **VNET 統合** をクリック

  <img src="images/app-vnet-integration-02.png" />

- **＋ VNet の追加** をクリック

  <img src="images/app-vnet-integration-03.png" />

- 統合する仮想ネットワークを選択し **OK** をクリック

  - **サブスクリプション**: ワークショップで使用しているサブスクリプション

  - **仮想ネットワーク**: ワークショップで使用している仮想ネットワーク

  - **サブネット**: 既存のものを選択、Subnet-2

    <img src="images/app-vnet-integration-04.png" />

- 選択した仮想ネットワーク、サブネットが追加されていることを確認

  <img src="images/app-vnet-integration-05.png" />

<br />

- App Service の管理ブレードから **構成** を選択

- **＋ 新しいアプリケーション設定** をクリック

- アプリケーション設定を追加

  - **名前**: WEBSITE_VNET_ROUTE_ALL

  - **値**: 1

    <img src="images/add-app-configuration-1.png" />

- **保存** をクリック

  <img src="images/add-app-configuration-2.png" />

- アプリケーションを再起動する旨のメッセージが表示されるため **続行** をクリック

  <img src="images/add-app-configuration-3.png" />

<br />

- **概要** ページの URL をクリックしアプリケーションを表示

  <img src="images/new-app-configuration-6.png" />

  ※ アプリケーションを介してデータベースへアクセスできることを確認

<br />

## Exercise 3: アプリケーションの保護

<img src="images/exercise3.png" />

<br />

### Task 1: Front Door の作成・構成

**Front Door の作成**

- Azure ポータルのトップ画面から **＋ リソースの作成** をクリック

- テキストボックスに **front door** と入力

  表示される候補より **Front Door and CDN Profiles** を選択

  <img src="images/create-front-door-01.png" />

- **作成** をクリック

  <img src="images/create-front-door-02.png" />

- **オファリングの比較** 画面で **Azure Front Door** と **カスタム作成** を選択し **Continue to create a Front Door** をクリック

  <img src="images/create-front-door-03.png" />

- **基本** タブでサブスクリプション、リソース グループ、名前、レベルを指定

  - **サブスクリプション**: ワークショップで使用しているサブスクリプション

  - **リソース グループ**: ワークショップで使用しているリソース グループ

  - **名前**: 任意（fd-workshopxx など）

  - **レベル**: Premium

    <img src="images/create-front-door-04.png" />

- **次へ: シークレット >** をクリック

- 証明書の追加は行わず **次へ: エンドポイント >** をクリック

- **エンドポイントの追加** をクリック

  <img src="images/create-front-door-05.png" />

- **エンドポイント名** を入力し **追加** をクリック

  - **エンドポイント名**: 任意

  - **状態**: このエンドポイント名を有効にするにチェック

    <img src="images/create-front-door-06.png" />

- **ルートの追加** をクリック

  <img src="images/create-front-door-07.png" />

- **新しい配信元グループを追加する** をクリック

  <img src="images/create-front-door-08.png" />

- 名前を入力し **配信元の追加** をクリック

  <img src="images/create-front-door-09.png" />

- 必要事項を設定し **追加** をクリック

  - **名前**: 任意（WebApp1 など）

  - **配信元の種類**: App Services

  - **ホスト名**: ワークショップで使用している App Service を選択

  - **HTTP ポート**: 80

  - **HTTPS ポート**: 443

  - **優先順位**: 1

  - **重み**: 1000

  - **プライベート リンク**: プライベート リンク サービスを有効にするにチェック

  - **Region**: App Service を展開しているリージョン

  - **ターゲット サブ リソース**: sites

  - **要求メッセージ**: 任意（Private link service from AFD）

  - **状態**: この元を有効にするにチェック

    <img src="images/create-front-door-10.png" />

- 配信元に App Service が追加されていることを確認し **追加** をクリック

  <img src="images/create-front-door-11.png" />

- 配信元グループが追加されていることを確認し **追加** をクリック

  <img src="images/create-front-door-12.png" />

- **ポリシーの追加** をクリック

  <img src="images/create-front-door-13.png" />

- 名前を入力し **新規作成** をクリック

  <img src="images/create-front-door-14.png" />

- **名前** を入力し **作成** をクリック

  <img src="images/create-front-door-15.png" />

- **ドメイン** に先の手順で追加したエンドポイントを選択し **保存** をクリック

  <img src="images/create-front-door-16.png" />

- ルート、ポリシーが追加されていることを確認し **確認および作成** をクリック

  <img src="images/create-front-door-17.png" />

- 設定内容を確認し **作成** をクリック

  <img src="images/create-front-door-18.png" />

<br />

**WAF ポリシーのモード変更**

- 作成した Front Door の管理ブレードへ移動

- **概要** ページのポリシー名をクリック

- WAF ポリシーを選択

  <img src="images/waf-policy-01.png" />

- **防止モードに切り替える** をクリック

  <img src="images/waf-policy-02.png" />

  ※ WAF の動作を規則に合致した侵入や攻撃と見なされるリクエストをブロックするよう変更

<br />

**Front Door の診断設定**

- Front Door の管理ブレードで **診断設定** を選択

- **＋ 診断設定を追加する** をクリック

  <img src="images/front-door-diag-1.png" />

- ログ、メトリックを Log Analytics ワークスペースへ送信するよう設定

  - **診断設定の名前**: 任意（diag-workshop-xx など）

  - **ログ**: allLogs を選択

  - **メトリック**: AllMetrics を選択

  - **宛先の詳細**: Log Analytics ワークスペースへの送信をチェックし、Log Analytics ワークスペースを選択

    <img src="images/front-door-diag-2.png" />

- **保存** をクリックし、設定内容を保存

<br />

**App Service のプライベート エンドポイントの承認**

- App Service の管理ブレードから **ネットワーク** を選択

- **プライベート エンドポイント** をクリック

  <img src="images/app-private-endpoint-1.png" />

- Front Door の配信元グループへの追加時に設定したプライベート エンドポイントが作成されているため、選択し **承認** をクリック

  <img src="images/app-private-endpoint-2.png" />

- 確認のメッセージが表示されるため **はい** をクリック

  <img src="images/app-private-endpoint-3.png" />

- 接続状態が Approved に変更されることを確認

  <img src="images/app-private-endpoint-4.png" />

- 受信トラフィックのプライベート エンドポイント、送信トラフィックの VNET 統合がオンになっていることを確認

  <img src="images/app-private-endpoint-5.png" />

- 管理ブレードの概要タブから URL をクリック

  <img src="images/app-service-summary.png" />

- プライベート エンドポイントの有効化によりインターネットからのアクセスが拒否されることを確認

  <img src="images/error-403-forbidden.png" />

<br />

**アプリケーションの動作確認**

- Front Door の管理ブレードで **概要** を表示

- Endpoint hostname をコピーし、新しいタブでアプリケーションにアクセス

  <img src="images/confirm-web-app-1.png" />

  ※ Front Door 経由ではインターネットでアプリケーションへアクセス可であることを確認

- 検索ワードに SQL インジェクションや XSS 攻撃を行うコードを入力し検索を実行

  - SQL インジェクション

    ```
    ' OR 1 = 1 --
    ```

  - XSS 攻撃

    ```
    ' -- <script>alert('1');</script>
    ```

- リクエストがブロックされることを確認

  <img src="images/confirm-web-app-3.png" />

<br />

### Task 2: ログの確認

- Front Door の管理ブレードで **ログ** を選択

- クエリを実行し WAF ポリシーでブロックしたログを抽出

  ```
  AzureDiagnostics 
  | where Category == "FrontDoorWebApplicationFirewallLog"
  | where policyMode_s == "prevention"
  ```

  <img src="images/prevention-log.png" />

  ※ 結果のレコードを展開し詳細を確認

- リクエストをブロックした理由と数を表示するクエリを実行

  ```
  AzureDiagnostics 
  | where Category == "FrontDoorWebApplicationFirewallLog"
  | where policyMode_s == "prevention"
  | summarize count() by details_msg_s
  ```

  <img src="images/front-door-log-3.png" />

- HTTP ステータス コードごとのリクエスト数を表示するクエリを実行

  ```
  AzureDiagnostics 
  | where Category == "FrontDoorAccessLog"
  | summarize count() by httpStatusCode_s
  | render piechart 
  ```

  <img src="images/front-door-log-1.png" />

- １時間ごとのリクエスト数を表示するクエリを実行

  ```
  AzureDiagnostics 
  | where Category == "FrontDoorAccessLog"
  | summarize count() by httpStatusCode_s, bin(TimeGenerated, 1h)
  | render barchart 
  ```

  <img src="images/front-door-log-2.png" />

<br />

## Exercise 4: マルチリージョン展開による高可用性構成

<img src="images/exercise4.png" />

<br />

### Task 1: リソースの作成

<br />

**リソース グループの作成**

- Azure ポータルのトップ画面で **＋ リソースの作成** をクリック

- 検索ワードに **resource group** と入力し、表示される候補より **リソース グループ** を選択

  <img src="images/create-resource-group-1.png" />

- **作成** をクリック

  <img src="images/create-resource-group-2.png" />

- 必要項目を設定し **確認および作成** をクリック

  - **サブスクリプション**: ワークショップで使用しているサブスクリプション

  - **リソース グループ**: 任意（rg-worksohp-xx など）

  - **リージョン**: Japan East を選択

    <img src="images/create-resource-group-3.png" />

- **作成** をクリックし、指定した内容でリソース グループを追加

  <img src="images/create-resource-group-4.png" />

<br />

**仮想ネットワークの作成**

- Azure ポータルのトップ画面で **＋ リソースの作成** をクリック

- 検索ワードに **virtual network** と入力し、表示される候補より **Virtual network** を選択

  <img src="images/new-vnet-1.png" />

- **作成** をクリック

  <img src="images/new-vnet-2.png" />

- 必要事項を設定し **次: IP アドレス >** をクリック

  - **サブスクリプション**: ワークショップで使用しているサブスクリプション

  - **リソース グループ**: 先の手順で作成した新しいリソース グループ

  - **名前**: 任意（vnet-2 など）

  - **地域**: (Asia Pacific) Japan East が選択されていることを確認

    <img src="images/new-vnet-3.png" />

- IPv4 アドレス空間、サブネットを指定し **次: セキュリティ >** をクリック

  - **IPv4 アドレス空間**: 任意（10.2.0.0/16）

  - **サブネット名**: Subnet-1 / **サブネット アドレス範囲**: 任意（10.2.1.0/24）

  - **サブネット名**: Subnet-2 / **サブネット アドレス範囲**: 任意（10.2.2.0/24）

    <img src="images/new-vnet-4.png" />

    ※ サブネットは **＋ サブネットの追加** をクリックし、必要事項を入力

    <img src="images/new-vnet-5.png" />

- セキュリティ設定は既定のまま **確認および作成** をクリック

  <img src="images/new-vnet-6.png" />

- **作成** をクリックし、指定した内容で仮想ネットワークを追加

  <img src="images/new-vnet-7.png" />

<br />

**Web アプリの作成**

- Azure ポータルのトップ画面で **＋ リソースの作成** をクリック

- **Web アプリ** の **作成** をクリック

  <img src="images/new-app-service-1.png" />

- **基本** タブでアプリ名、ランタイム、App Service プラン等を設定し **次: デプロイ >** をクリック

  - **サブスクリプション**: ワークショップで使用しているサブスクリプション

  - **リソース グループ**: 先の手順で作成した新しいリソース グループ

  - **インスタンスの詳細**

    - **名前**: 任意 (app-workshop-xxx など、ユニークとなる名前)

    - **公開**: コード

    - **ランタイム スタック**: .NET 6 (LTS)

    - **オペレーティング システム**: Windows

    - **地域**: (Asia Pacific) Japan East が選択されていることを確認

  - **App Service プラン**

    - **Windows プラン**: 新規作成をクリックしプラン名（plan-workshop-xx など）を入力

      <img src="images/new-app-service-3.png" />

    - **SKU とサイズ**: Standard S1

      ※ Standard S1 が選択されていない場合は、**サイズを変更します** をクリックし S1 を選択

      <img src="images/new-app-service-4.png" />

  - **ゾーン冗長**: 無効

  <img src="images/new-app-service-2.png" />

- **デプロイ** タブは既定（無効化）のまま **次: ネットワーク (プレビュー) >** をクリック
  
  <img src="images/new-app-service-5.png" />

- **ネットワーク (プレビュー)** タブでネットワーク インジェクションで **オフ** が設定されていることを確認

  **次: 監視 >** をクリック 

  <img src="images/new-app-service-6.png" />

- **監視** タブで **Application Insights を有効にする** を **いいえ** に変更

  **確認および作成** をクリック

  <img src="images/new-app-service-7.png" />

- **作成** をクリックし、指定した内容で Web アプリを展開

  <img src="images/new-app-service-8.png" />

※ Web アプリの展開完了後に主催者からアプリを配信します

<br />

### Task 2: VNet Peering の構成

- 新しく追加した仮想ネットワークの管理ブレードへ移動

- **ピアリング** を選択し **＋ 追加** をクリック

  <img src="images/vnet-peering-1.png" />

- 双方向のピアリング名を入力し、既存の仮想ネットワークを選択したのち **追加** をクリック

  <img src="images/vnet-peering-2.png" />

<br />

### Task 3: Cosmos DB グローバル分散

- Cosmos DB の管理ブレードで **データをグローバルにレプリケートする** を選択

  <img src="images/replicate-db-1.png" />

- **リージョンの追加** をクリック

- 読み取りリージョンに **Japan East** を選択し **OK** をクリック

  <img src="images/replicate-db-2.png" />

- **保存** をクリック

  <img src="images/replicate-db-3.png" />

- **自動フェールオーバー** をクリック

- **自動フェールオーバーの有効化** を **オン** に変更し **OK** をクリック

  <img src="images/automatic-failover.png" />

- **プライベート エンドポイント接続** を選択

- 既存のプライベート エンドポイントをクリックし **DNS の構成** を選択

  <img src="images/replicate-db-4.png" />

  ※ 追加したリージョン用の IP アドレス、FQDN が作成されていることを確認

<br />

### Task 4: Cosmos DB Private Endpoint の追加

- Cosmos DB の管理ブレードから **プライベート エンドポイント接続** を選択

- **＋ プライベート エンドポイント** をクリック

- **基本** タブでサブスクリプション、リソース、名前、地域を指定

  - **サブスクリプション**: ワークショップで使用しているサブスクリプション

  - **リソース グループ**: ワークショップで使用しているリソース グループ

  - **名前**: 任意（pl-cosmos-workshop-xxx など）

  - **Network Interface Name**: 任意（既定のままで OK）

  - **地域**: 新しく作成した仮想ネットワークの地域を選択

    <img src="images/replicate-db-5.png" />

- **リソース** タブでプライベート エンドポイントを作成するリソースを指定

  - **サブスクリプション**: ワークショップで使用しているサブスクリプション

  - **リソースの種類**: Microsoft.AzureCosmosDB/databaseAccounts

  - **リソース**: ワークショップで使用している Cosmos DB アカウント

  - **対象サブ リソース**: Sql

    <img src="images/cosmos-private-endpoint-03.png" />

- **仮想ネットワーク** タブでプライベート エンドポイントを作成する仮想ネットワーク、サブネットを選択

  - **仮想ネットワーク**: Japan East へ作成した仮想ネットワーク

  - **サブネット**: Subnet-1 を選択

    <img src="images/replicate-db-6.png" />

- DNS の構成で追加したリソース グループに新規でプライベート DNS ゾーンが作成されることを確認

  <img src="images/replicate-db-7.png" />

- タグは既定のまま、プライベート エンドポイントを作成

- 新たに追加したプライベート エンドポイントの DNS 構成を確認

  <img src="images/replicate-db-8.png" />

  ※ グローバル エンドポイントと Cosmos DB アカウントとレプリカ展開されているリージョン用の IP アドレス、FQDN が作成

<br />

### Task 5: App Service の構成

**アプリケーション設定**

- 新たに展開した App Service の管理ブレードから **構成** を選択

- **＋ 新しいアプリケーション設定** をクリック

  <img src="images/new-app-configuration-1.png" />

- 名前、値を入力しのアプリケーション設定を追加

  - **名前**: CosmosDbEndpoint
  
  - **値**: Cosmos DB アカウントの URI

    <img src="images/new-app-configuration-2.png" />

- 同様の手順で３つアプリケーション設定を追加

  - **名前**: AuthorizationKey　/　**値**: Cosmos DB アカウントのプライマリ キー

  - **名前**: WEBSITE_VNET_ROUTE_ALL　/　**値**: 1

  - **名前**: ServerRegion　/　**値**: JapanEast

- **保存** をクリック

  <img src="images/new-app-configuration-3.png" />

- 再起動を行うメッセージが表示されるので **続行** をクリック

  <img src="images/new-app-configuration-4.png" />

<br />

**App Service の VNet 統合**

- 新たに展開した App Service の管理ブレードから **ネットワーク** を選択

- **VNET 統合** をクリック

  <img src="images/app-vnet-integration-01.png" />

- **＋ VNet の追加** をクリック

  <img src="images/app-vnet-integration-03.png" />

- Japan East へ作成した仮想ネットワークとその Subnet-2 を選択

- 選択した仮想ネットワークが追加されることを確認

  <img src="images/new-app-configuration-5.png" />

<br />

**Application Insights の追加**

- 新たに展開した App Service の管理ブレードの **Application Insights** を選択

- **Application Insights を有効にする** をクリック

  <img src="images/add-application-insights-1.png" />

- 新しいリソースの設定を行い **適用** をクリック

  - **新しいリソースの名前**: 任意 (appi-workshop-xxx など)

  - **場所**: (Asia Pacific) Japan East

  - **Log Analytics ワークスペース**: ワークショップで使用している Log Analytics ワークスペースを選択

    <img src="images/add-application-insights-2.png" />

<br />

**アプリケーションの動作確認**

- 新たに展開した App Service の管理ブレードの概要タブで URL をクリックしアプリケーションへアクセスを実行

  <img src="images/new-app-configuration-6.png" />

  ※ 正常にアプリケーションが動作することを確認

- App Service の管理ブレードで **Application Insights** を選択

- **Application Insights データの表示** をクリック

  <img src="images/add-application-insights-3.png" />

- **パフォーマンス** を選択

  ※ アプリケーションの各操作の数や平均実行時間が表示

- **操作の選択** から **GET /** を選択、**詳細の表示 ...** にある **サンプル** ボタンをクリック

  <img src="images/add-application-insights-4.png" />

- 画面右に表示される **操作のサンプルを選択** ペインから応答コード 200 の任意の項目をクリック

- エンド ツー エンド トランザクションの詳細画面で Cosmos DB への接続を確認

  <img src="images/add-application-insights-5.png" />

<br />

### Task 6: Front Door 配信元グループへの App Service の追加

**配信元グループの更新**

- Front Door の管理ブレードから **配信元グループ** を選択

- 先の手順で追加した配信元グループをクリック

  <img src="images/add-origin-1.png" />

- **＋ 配信元の追加** をクリック

  <img src="images/add-origin-2.png" />

- 配信元として新たに展開した App Service を選択、優先順位を 2 に設定し **追加** をクリック

  - **名前**: 任意（WebApp2 など）

  - **配信元の種類**: App Services

  - **ホスト名**: 新たに展開した App Service を展開

  - **HTTP ポート**: 80

  - **HTTPS ポート**: 443

  - **優先順位**: 2

  - **重み**: 1000

  - **プライベート リンク**: プライベート リンク サービスを有効にするにチェック

  - **Region**: Japan East

  - **ターゲット サブ リソース**: sites

  - **要求メッセージ**: 任意（Private link service from AFD など）

  - **状態**: この元を有効にするにチェック

    <img src="images/add-origin-3.png" />

- 配信元のホスト名に App Service が追加されたことを確認し **更新** をクリック

  <img src="images/add-origin-4.png" />

<br />

**App Service のプライベート エンドポイントの承認**

- App Service の管理ブレードから **ネットワーク** を選択

- **プライベート エンドポイント** をクリック

- Front Door の配信元グループへの追加時に設定したプライベート エンドポイントが作成されているため、選択し **承認** をクリック

  <img src="images/new-app-configuration-7.png" />

- 確認のメッセージが表示されるため **はい** をクリック

- 接続状態が Approved に変更されることを確認

  <img src="images/new-app-configuration-8.png" />

- 受信トラフィックのプライベート エンドポイント、送信トラフィックの VNET 統合がオンになっていることを確認

  <img src="images/new-app-configuration-9.png" />

- 管理ブレードの概要タブから URL をクリック

- プライベート エンドポイントの有効化によりインターネットからのアクセスが拒否されることを確認

  <img src="images/error-403-forbidden.png" />

<br />

**アプリケーションの動作確認**

- Front Door の管理ブレードで **概要** を表示

- Endpoint hostname をコピーし、新しいタブでアプリケーションにアクセス

  <img src="images/confirm-web-app-1.png" />

  ※ Front Door 経由ではインターネットでアプリケーションへアクセス可であることを確認

- 既存の App Service の管理ブレードの概要ページを表示

- 画面上部の **停止** をクリック

  <img src="images/failover-app-1.png" />

- 停止を確認するメッセージが表示されるため **はい** をクリック

  <img src="images/failover-app-2.png" />

- アプリケーションが停止されたことを確認

  <img src="images/failover-app-3.png" />

- Front Door の Endpoint hostname でアプリケーションへアクセス

  <img src="images/failover-app-4.png" />

  ※ アプリケーションへ正常にアクセスできることを確認

- Application Insights で新たな App Service へリクエストが行われていることを確認

  <img src="images/failover-app-5.png" />

