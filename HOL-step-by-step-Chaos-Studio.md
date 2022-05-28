![Microsoft Cloud Workshop](images/ms-cloud-workshop.png)

Protect and Scale Apps Hands-on lab  
June 2022

<br />

### 参考情報

- [Azure Chaos Studio のドキュメント](https://docs.microsoft.com/ja-jp/azure/chaos-studio/)

<br />

## リソースの展開

## Exercise ★: Chaos Studio を使った実験 (ToDo: エクササイズ番号を後で入れる)

### Task 1: ターゲット(Cosmos DB)のオンボード

- Azure ポータルのトップ画面から **検索バー** のテキストボックスに **Chaos Studio** と入力

- 表示される候補より **Chaos Studio** を選択

　<img src="images/chaos-studio-1.png" />

- **対象** をクリック

　<img src="images/chaos-studio-2.png" />

- ワークショップで使用中のサブスクリプション、リソースグループを選択し、Cosmos DB にチェック

　<img src="images/chaos-studio-3.png" />

- **ターゲットを有効にする**をクリックし、**サービス直接ターゲット（すべてのリソース）を有効にする**をクリック

　<img src="images/chaos-studio-4.png" />

<br />

### Task 2: 実験作成

- Azure ポータルのトップ画面から **検索バー** のテキストボックスに **Chaos Studio** と入力

- 表示される候補より **Chaos Studio** を選択

　<img src="images/chaos-studio-1.png" />

- **実験** をクリック

　<img src="images/chaos-studio-5.png" />

- **+ 作成** をクリック

- 実験の作成画面が表示

    - 必要項目を入力し **次へ: 実験デザイナー >** をクリック

      - サブスクリプション： ワークショップで使用中のサブスクリプションを選択

      - リソース グループ： ワークショップで使用するリソース グループを選択

      - 名前: experiment-001（任意）

      - 場所: East US

　<img src="images/chaos-studio-6.png" />

- 実験デザイナーが表示

    - 必要項目を入力し **+ アクションの追加** をクリック

      - ステップ： Inject Incident（任意）

      - ブランチ: Inject CosmosDB Failover（任意）

　<img src="images/chaos-studio-7.png" />

- フォールトの追加が表示

    - フォールト: **CosmosDB Failover**

    - 以下のパラメーターを入力し、**次へ: ターゲット リソース** をクリック

        - Duration (minutes): **10**

        - readRegion: **West US**

　<img src="images/chaos-studio-8.png" />

- ターゲットリソースが表示

    - ワークショップで使用する Cosmos DB を選択し **追加** をクリック

　<img src="images/chaos-studio-9.png" />

- **確認および作成**をクリック

- 表示内容を確認し問題がなければ **作成** をクリック

　<img src="images/chaos-studio-10.png" />

### Task 3: マネージド ID への権限付与

- Azure ポータルのトップ画面から **検索バー** のテキストボックスに **Azure Cosmos DB** と入力

- 表示される候補より **Azure Cosmos DB** を選択

　<img src="images/chaos-studio-11.png" />

- ワークショップで使用中の Cosmos DB を選択

　<img src="images/chaos-studio-12.png" />

- **アクセス制御（IAＭ）** を選択し、**+ 追加**、**ロールの割り当ての追加**を選択

　<img src="images/chaos-studio-13.png" />

- **Cosmos DB 演算子**を入力し、**Cosmos DB 演算子**を選択し、**次へ**を選択

　<img src="images/chaos-studio-14.png" />

- 必要項目を入力し **次へ** をクリック

    - アクセスの割り当て先: **マネージド ID**
    - メンバー
        - サブスクリプション: ワークショップで使用中のサブスクリプションを選択
        - マネージド ID: **カオス実験**
        - 選択: **experiment-001**（作成した実験名）

　<img src="images/chaos-studio-15.png" />

 - 表示内容を確認し問題がなければ **レビューと割り当て** をクリック

 　<img src="images/chaos-studio-16.png" />

- ワークショップで使用中の Cosmos DB を選択

　<img src="images/chaos-studio-12.png" />

- **データをグローバルにレプリケートする**を選択し、読み取りリージョンに**West US**を追加し、**保存**をクリック

　<img src="images/chaos-studio-19.png" />


### Task 4: 実験実行

- Azure ポータルのトップ画面から **検索バー** のテキストボックスに **Chaos Studio** と入力

- 表示される候補より **Chaos Studio** を選択

　<img src="images/chaos-studio-1.png" />

- **実験** をクリックし、作成済みの実験をクリック

　<img src="images/chaos-studio-17.png" />

- **▶ 開始**をクリックし、実験を開始

　<img src="images/chaos-studio-18.png" />

### Task 5: 実験結果確認

**※アクティビティログが出力されるまで、実験開始から約5分ほどかかります。**

<br>

- ワークショップで使用中の Cosmos DB を選択

　<img src="images/chaos-studio-12.png" />

- アクティビティログを選択し、**Manual Failover** を選択

　<img src="images/chaos-studio-20.png" />

- **変更履歴（プレビュー）** を選択し、**properties.writeLocations** を選択

　<img src="images/chaos-studio-21.png" />

- **locationName** が **West US** に変更されていることを確認

　<img src="images/chaos-studio-22.png" />