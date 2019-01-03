using System;
using System.Text;
using System.IO;

// これは KJ_dict.dll のSearchメソッドを使用するサンプルです。
//
//   コンパイルの例:   csc testmain.cs /reference:KJ_dict.dll


//  KJ_dictクラスのDictSearchは、辞書をひいて結果を返します

namespace HYAM.KJ_dict{
    
class testmain{

    
    static  void Main( ) {  
        
                                               
        //  Open dictionary
        KJ_dict.DictOpen("KJ_dict.yml");
                                    
        String  search_word = "あ";   // 検索語

        // 検索処理          
        //                                         検索タイプ           検索語
        SearchResult result = KJ_dict.DictSearch( SearchType.forward,  search_word );
                                                    //   full
                                                    //   forward  
                                                    //   backward
                                                    //   part
        
        
        Console.WriteLine("--------------------------------------");
        Console.WriteLine( "return_code=" + result.return_code  );
                    // result.return_code  0:語が見つかった  -1:見つからなかった
                    //                     1:該当する語が100個を超えた(100個までしか返しません)     
        
        // 結果を出力するファイルの準備
        FileStream fs   = new FileStream("result.txt", FileMode.Create, FileAccess.Write);
        StreamWriter w  = new StreamWriter(fs, Encoding.UTF8);


        // 検索結果はresult.search_dataに配列として入っています

        // 一致した語の数だけループ
        foreach ( DocumentData docdata in result.documents  ){
          //  w.Write("key2:" + docdata.GetKey(false)  + "\n"  ) ;    // 日本語
            w.Write("key2:" + docdata.GetData("key2")  + "\n"  ) ;    // 日本語
          //  w.Write("key1:" + docdata.GetKey(true)   + "\n\n") ;    // 韓国語
            w.Write("key1:" + docdata.GetData("key1")   + "\n\n") ;    // 韓国語
        }       
         
        w.Close();
        
        // 結果は result.txtに utf-8で出力されます。
    }

}

}


