using System;
using System.Text;
using System.IO;

// これは KJ_dict.dll のStringTransクラスとそのTransメソッドを使用するサンプルです。
//   コンパイルの例:   csc testmain2.cs /reference:KJ_dict.dll

//  StringTransクラスのTransは、渡された文章を翻訳して結果を返します

namespace HYAM.KJ_dict{
    
class testmain2{
    
    static  void Main( ) {  
        StringTrans     stringTranslator;
        
        // 結果を出力するファイルの準備
        FileStream   fs = new FileStream("result2.txt", FileMode.Create, 
                                         FileAccess.Write);
        StreamWriter w  = new StreamWriter(fs, Encoding.UTF8);

        // コンスタラクタ
        stringTranslator = new StringTrans();

        
        KJ_dict.inputIsHangul = true;
        string input = "가상화 솔루션에 많은 관심을 보여왔다";   // 翻訳したい文章
                       
//        string input = "会社員が,やむを得ない事情で犬と働いている";   
//                          翻訳したい文章

              // 形態素解析まともにやっていないので、
              // ひらがなが続く場合は「,」,で切った方がいい (^^;;
              // 漢字・ひらがな・カタカナの切り替えがあれば大丈夫なんですが...
        
        
             
        // 変換実行
        string output = stringTranslator.Trans( input ) ;

        w.Write(input   + "\n"  ) ;    
        w.Write("↓\n"          ) ;    
        w.Write(output  + "\n"  ) ;   // 翻訳結果
         
        w.Close();
        
        // 結果は result2.txtに utf-8で出力されます。
    }

}

}


