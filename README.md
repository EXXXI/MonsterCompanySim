# MonsterCompanySim

## 概要

モンスターカンパニー用の戦闘シミュレータです

## 更新状況

対応モンカニVer.：Ver.7.22.0対応

最終の更新：2025/8/1 Ver.7.22.0対応(2.5部8話追加、ひだね強化、敵パーニー調整)

詳細や履歴は[リリースノート](ReleaseNotes.md)を参照

## 使い方

### 起動方法

- MonsterCompanySim.zipをダウンロード
- MonsterCompanySim.zipを解凍
- 中にあるMonsterCompanySim.exeをダブルクリック

### 入力欄説明

- 上3人分が敵社員入力欄、下3人分が味方社員入力欄です
- ステージから選択することもできます
  - 需要の多そうな終盤のステージのみ用意
- タゲは左から1,2,3です
- 敵社員入力欄にある、スキル無効のチェックをつけると、その社員のスキルの計算を行なわなくなります
  - たまにいるスキルが動いてない敵社員用です
  - ただし、ツール開発者が把握している以下社員はチェックをつけなくても敵側でのみ該当スキルやステータスを自動で調整するので、あまり出番はないはず
    - 1部：泣き虫エルフ・ボンファイター・雪の子・テングガール・クロコ・シロナ・ノーワーカー・アルアナ・ヒミコ
    - 2部：アレグリアス・キラif・双極マザー・ラビリンス・奈々・岬・アサインバイト・ナーガ・ミコ・ヒコ・キコ・パーニー

### 機能説明

- 機能1：勝率計算(ターゲット指定)
  - 入力された敵社員と味方社員とタゲの情報を元に、勝率と最良の戦闘結果を表示します
  - タゲに影響を与えるスキルは無視され、入力したタゲが優先されます
- 機能2：勝率計算(ターゲットランダム)
  - 入力された敵社員と味方社員の情報を元に、勝率と最良の戦闘結果を表示します
- 機能3：要求Lv概算
  - 入力された敵社員と味方社員の情報を元に、その編成で勝利するための要求レベルを大まかに計算します
  - この機能で算出するLvは、「3人全員が同じLvの場合、そのLvでギリギリ勝率が0じゃなくなる」ラインです
  - 1人だけ高Lvにするなどすれば、もっと簡単に勝てる可能性が高いため、あくまでも目安です
- 機能4：編成検索
  - 入力された敵社員の情報を元に、勝ち目のある編成をリストアップします
    - 味方社員欄は空でいいです
  - 敵社員情報の他に、ステージが1部なのか2部なのか、現在のLv上限はどこかの情報を入力する必要があります
    - 入力欄は編成検索ボタンの左です
    - 部はプルダウンで、Lv上限は半角数字で109999とか入れてください
  - 多少工夫はしましたが基本総当たりなので重めです
    - 編成検索ボタンが灰色のうちは計算中です
    - 開発者のPCでは数十秒かかります
  - 重いので検索対象の社員を絞っています
    - 初期状態ではLXR以上+アルアナ+属性2.5倍勢だけ有効になっています
    - 「検索対象設定」タブで確認&再設定できます
      - 当然ながら対象を増やせば増やすほど重くなるので注意
      - 「必ず編成に入れる」にチェックを入れるとその社員を含めた編成だけを計算します
  - 検索結果を右クリックすると上部の味方欄に転送できます

## 注意点

- Windowsでしか動きません(たぶんWin7以上ならOK)
- .Netのインストールが必要です(無ければ起動時に案内されるはず)
  - デスクトップランタイムで動くはず
- ファイル入出力を行うので環境によってはウィルス対策ソフトが文句を言ってきます
- もろもろの計算式は想像で補っている上、演算誤差を考慮せずに組んでいるので多少誤差は出ます
  - 特に低レベル帯では顕著です
  - なので低レベル帯はこんなの使わずレベリングして押しつぶすのが吉

## ライセンス

MIT License

### ↑このライセンスって何？

こういう使い方までならOKだよ、ってのを定める取り決め

今回のは大体こんな感じ

- 誰でもどんな目的でも好きに使ってOK
- でもこれのせいで何か起きても開発者は責任取らんよ
- 改変や再配布するときはよく調べてルールに従ってね

## 使わせていただいたOSS(+必要であればライセンス)

### System.Text.Json

プロジェクト：<https://dot.net/>

ライセンス：<https://github.com/dotnet/runtime/blob/main/LICENSE.TXT>

### Prism.Wpf

プロジェクト：<https://github.com/PrismLibrary/Prism>

ライセンス：<https://www.nuget.org/packages/Prism.Wpf/8.1.97/license>

### ReactiveProperty

プロジェクト：<https://github.com/runceel/ReactiveProperty>

ライセンス：<https://github.com/runceel/ReactiveProperty/blob/main/LICENSE.txt>
