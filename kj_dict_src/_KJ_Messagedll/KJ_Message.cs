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
  // �R���X�g���N�^
  public KJ_Message(){

    this.rscManager = new ResourceManager("KJ_Message", this.GetType().Assembly) ;
    
    KJ_Message.cultureName = CultureInfo.CurrentCulture.Name  ;
    
    if(KJ_Message.cultureName == "ja-JP" || KJ_Message.cultureName == "ko-KR" ){
        // nop
    }else{
        // ja-JP ko-KR�łȂ��ꍇ�͉p��ɂ���B
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
     
     // �����I�ɕʌ���ݒ�
     Thread.CurrentThread.CurrentUICulture = new CultureInfo(country);
     
     // static�ϐ��X�V
     KJ_Message.cultureName = cultureName;
     
  }
  
  //--------------------------------------------------------------------
  public string Get(string target){
    string msg;
    
    // �܂���default���P�[���Ń��b�Z�[�W��T���A���߂Ȃ�p��ɂ��� 
    try{
        
        // �����I�ɕʌ���ݒ�
        string country = KJ_Message.cultureName.Substring(0, 2); 
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(country);
        
        msg = rscManager.GetString(target);

    } catch {
        // rscManager.GetString�Ō�����Ȃ������ꍇ�������ɗ���̂�
        // 1��ł�������Ȃ��ƈȍ~�p�ꕶ����ɂȂ��Ă��܂��B

        // �Y�����b�Z�[�W���Ȃ���΋��Ԃ�
        msg="";
    }

    return  msg;
  }
}


} // end of namespace
