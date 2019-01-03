// Copyright 2004, 2011 hyam <hyamhyam@gmail.com>
// and it is under GPL
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2, or (at your option)
// any later version.
 
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Globalization ;
using System.Windows.Forms;

#if !EDICT
using HYAM.Lingua ;
#endif

namespace HYAM.KJ_dict
{
    
    //-------------------------------------------------------------------------
    // 
    public class Pos{
        
        private static KJ_Message msg;
        
        //--------------------------------------------------------------------
        // static constructer   
        static Pos(){
            msg = new KJ_Message();
        }    
        
        static public string conjugationName(string conjugation){
            string name = msg.Get(conjugation) ;
            if(name != ""){
                return name;
            }
            return conjugation ;
        }
        //------------------------------------------------------------
        // static public method 
#if !EDICT             
        // for KJ_dict
        static public string Name(string pos){
             
            if(pos == ""){
                return "";
            }
            
            int periodPos = pos.IndexOf(".");
            if(periodPos < 0){
                // "."なし
                return msg.Get("posname_" + pos);  // posname_v もここ
            }
            
            if( pos.StartsWith("pn.")){
                string pnname;
                pnname = msg.Get("posname_pn") ;

                string detail = msg.Get(pos);
                if(detail != ""){
                    pnname = pnname + " : " + detail ;
                }
                return pnname;
            }
            
            if( pos.StartsWith("noun.")){
                return msg.Get("posname_noun");
            }
            
            if( pos.StartsWith("pp.")){
                return msg.Get("posname_pp") ;
            }
            
            
            if(pos.StartsWith("verb.") || 
               pos.StartsWith("adj.")  || 
               pos.StartsWith("adjectivalverb.") ){
                
                string posConj = pos.Remove(0, pos.IndexOf(".") + 1 );
                if(posConj.EndsWith("2") || 
                   posConj.EndsWith("3") || 
                   posConj.EndsWith("4") ){
                    posConj = posConj.Remove(posConj.Length - 1, 1);
                }
                string conjugation = msg.Get("conjugation_" + posConj);

                string posname="";
                posname = pos.Substring(0, pos.IndexOf(".") ) ;
                posname = msg.Get("posname_" + posname);
                
                if(conjugation != ""){
                    return posname + " (" + conjugation + ")";
                }else{
                    return posname ;
                }
            }
            
            return "";
        }
#else
        // for KJ_Edict
        //   実験的に少し書いたがつかっていない [2011/01/15]
        static public string Name(string pos){
             
            pos = pos.Replace("(n;vs)", "名詞する");
            pos = pos.Replace("(n)", "名詞");
            pos = pos.Replace("(adj-i)", "形容詞");
            
            
            return pos;
        }

#endif

                
    } // end of class Pos 
    

} // End namespace KJdict

