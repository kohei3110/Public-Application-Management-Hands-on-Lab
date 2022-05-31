![Microsoft Cloud Workshop](images/ms-cloud-workshop.png)

Protect and Scale Apps Hands-on lab  
June 2022

<br />

### **Contents**

- [Advance preparatio: アプリケーションの動作確認](#advance-preparatio-アプリケーションの動作確認)

- [Exercise 1: Azure リソースの保護](#exercise-1-azure-リソースの保護)

  - [Task 1: Microsoft Defender for Cloud の有効化](#task-1-microsoft-defender-for-cloud-の有効化)

  - [Task 2: 不正アクセスの検出](#task-2-不正アクセスの検出)

- [Exercise 2: Cosmos DB アカウントへのアクセス制限](#exercise-2-cosmos-db-アカウントへのアクセス制限)

  - [Task 1: Cosmos DB アカウントへの Private Link の構成](#task-1-cosmos-db-アカウントへの-private-link-の構成)

  - [Task 2: App Service 仮想ネットワーク統合](#task-2-app-service-仮想ネットワーク統合)

- [Exercise 3: アプリケーションの保護](#exercise-3-アプリケーションの保護)

  - [Task 1: Front Door の作成・構成](#task-1-front-door-の作成・構成)

  - [Task 2: ログの確認](#task-2-ログの確認)

- [Exercise 4: 災害対策構成](#exercise-4-災害対策構成)

  - [Task 1: リソースの作成](#task-1-リソースの作成)

  - [Task 2: Cosmos DB グローバル分散](#task-2-cosmos-db-グローバル分散)

  - [Task 3: Cosmos DB Private Endpoint の追加](#task-3-cosmos-db-private-endpoint-の追加)

  - [Task 4: App Service の作成・構成](#task-4-app-service-の作成・構成)

  - [Task 5: Front Door 配信元グループへの App Service の追加](#task-5-front-door-配信元グループへの-app-service-の追加)

<br />

## 使用する環境

<img src="images/Azure-resources.png" />

<br />

## Advance preparatio: アプリケーションの動作確認

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

### Task 1: Cosmos DB アカウントへの Private Link の構成

- Cosmos DB の管理ブレードから **プライベート エンドポイント接続** を選択

- **＋ プライベート エンドポイント** をクリック

  <img src="images/cosmos-private-endpoint-01.png" />

- **基本** タブでサブスクリプション、リソース、名前、地域を指定

  - **サブスクリプション**: ワークショップで使用しているサブスクリプション

  - **リソース グループ**: ワークショップで使用しているリソース グループ

  - **名前**: 任意（pl-cosmos-workshop-xxx など）

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

### Task 1: Front Door の作成・構成

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

- 作成した Front Door の管理ブレードへ移動

- **概要** ページのポリシー名をクリック

- WAF ポリシーを選択

  <img src="images/waf-policy-01.png" />

- **防止モードに切り替える** をクリック

  <img src="images/waf-policy-02.png" />

  ※ WAF の動作を規則に合致した侵入や攻撃と見なされるリクエストをブロックするよう変更

<br />

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

- Front Door の管理ブレードで **概要** を表示

- Endpoint hostname をコピーし、新しいタブでアプリケーションにアクセス

  <img src="images/confirm-web-app-1.png" />

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

## Exercise 4: 災害対策構成

### Task 1: リソースの作成

<br />

**リソース グループの作成**

- Azure ポータルのトップ画面で **＋ リソースの作成** をクリック

<br />

**仮想ネットワークの作成**

<br />

**Web アプリの作成**

<br />

### Task 2: Cosmos DB グローバル分散

<br />

### Task 3: Cosmos DB Private Endpoint の追加

<br />

### Task 4: App Service の作成・構成

<br />

### Task 5: Front Door 配信元グループへの App Service の追加

<br />
