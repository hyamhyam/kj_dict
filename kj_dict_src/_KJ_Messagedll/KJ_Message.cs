// Copyright 2004, 2008 hyam <hhhyam@ybb.ne.jp>
// and it is under GPL
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.
using System.Resources;
using System.Threading;
using System.Globalization ;

namespace HYAM.KJ_dict
{


public class  KJ_Message{
  
  ResourceManager rscManager;


  static string cultureName;
  
  //--------------------------------------------------------------------
  // コンストラクタ
  public KJ_Message(){

    this.rscManager = new ResourceManager("KJ_Message", this.GetType().Assembly) ;
    
    KJ_Message.cultureName = CultureInfo.CurrentCulture.Name  ;
    
    if(KJ_Message.cultureName == "ja-JP" || KJ_Message.cultureName == "ko-KR" ){
        // nop
    }else{
        // ja-JP ko-KRでない場合は英語にする。
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
        KJ_Message.cultureName = "en-US";
    }
  }
  
  //--------------------------------------------------------------------
  public static string GetCultureName(){

    return KJ_Message.cultureName  ;
  }

  //--------------------------------------------------------------------
  // cultureName = "en-US" or "ko-KR" or "ja-JP" 
  public static void SetCultureName(string cultureName){

     string country = cultureName.Substring(0, 2); // "ko", "en", "jp"
     
     // 強制的に別言語設定
     Thread.CurrentThread.CurrentUICulture = new CultureInfo(country);
     
     // static変数更新
     KJ_Message.cultureName = cultureName;
     
  }
  
  //--------------------------------------------------------------------
  public string Get(string target){
    string msg;
    
    // まずはdefaultロケールでメッセージを探し、だめなら英語にする 
    try{
        
        // 強制的に別言語設定
        string country = KJ_Message.cultureName.Substring(0, 2); 
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(country);
        
        msg = rscManager.GetString(target);

    } catch {
        // rscManager.GetStringで見つからなかった場合もここに来るので
        // 1回でも見つからないと以降英語文字列になってしまう。

        // 該当メッセージがなければ空を返す
        msg="";
    }

    return  msg;
  }
}


} // end of namespace
