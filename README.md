# 等高線データ生成ライブラリ

## 概要

等高線を描画するためのデータを生成するライブラリ。データの生成順序は以下のとおり。

1. 標点と標高を記載したファイルを読み込む。
1. 標点データからドロネー三角形分割を行う。
1. ドロネー三角形を利用して等高線データを生成する。

## 参考文献

* <http://tercel-sakuragaoka.blogspot.com/2011/06/processingdelaunay.html>
* <http://tercel-sakuragaoka.blogspot.com/2011/06/processingdelaunay_3958.html>
* <http://www.kanenko.com/~kanenko/KOUGI/CompGeo/cpgeoc.pdf>
