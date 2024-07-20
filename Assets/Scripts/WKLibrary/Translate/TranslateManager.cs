using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml;
using WK.Utils;
using WK.Unity;//for Config

namespace WK
{
    namespace Translate
    {

        public class TranslateText
        {
            //public string jp;
            //public string eng;
            //public string kor;
            //public string sim;//簡体字
            //public string tra;//繁体字
            public string vie;
            //public TranslateText(string jp_, string eng_, string kor_, string sim_, string tra_, string vie_)
            public TranslateText(string vie_)
            {
                //jp = jp_;
                //eng = eng_;
                //kor = kor_;
                //sim = sim_;
                //tra = tra_;
                vie = vie_;
            }
        }

        public class TranslateManager : Singleton<TranslateManager>
        {
            private Dictionary<string, TranslateText> texts = new Dictionary<string, TranslateText>();

            ITagProcessor tagProcessor = null;

            //------------------------------------------------------------------------------
            public void Read(string path)
            {
                TextAsset text_asset = Resources.Load<TextAsset>(path);
                Debug.Assert(text_asset != null);

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(text_asset.text);

                var nsmgr = new XmlNamespaceManager(xmldoc.NameTable);
                nsmgr.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");

                XmlNodeList rows = xmldoc.GetElementsByTagName("Row");

                foreach (XmlNode row in rows)
                {
                    XmlNodeList datas = row.SelectNodes("ss:Cell/ss:Data", nsmgr);

                    string key = datas[0].InnerText;
                    //string jp = datas[1].InnerText;
                    //string eng = datas[2].InnerText;
                    //string kor = datas[3].InnerText;
                    //string sim = datas[4].InnerText;
                    //string tra = datas[5].InnerText;
                    string vie = datas[6].InnerText;

                    //改行コードを変換する
                    //手元のExcelを使ってxml形式で保存すると改行が\r(CR)になっているので、
                    //それを変換する
                    //jp = jp.Replace("\r", "\n");
                    //eng = eng.Replace("\r", "\n");
                    //kor = kor.Replace("\r", "\n");
                    //sim = sim.Replace("\r", "\n");
                    //tra = tra.Replace("\r", "\n");
                    vie = vie.Replace("\r", "\n");

                    //texts[key] = new TranslateText(jp, eng, kor, sim, tra, vie);
                    texts[key] = new TranslateText(vie);
                }

                //debug
                /* foreach (var v in texts) { */
                /*     Debug.Log( v.Key + "," + v.Value.eng + "," + v.Value.eng.Length + ":" + v.Value.jp + "\n" ); */
                /*     foreach (var c in v.Value.jp) { */
                /*         Debug.Log( c.ToString() ); */
                /*         Debug.Log( ((int)( c )).ToString() ); */
                /*         Debug.Log( ((int)( '\n' )).ToString() ); */
                /*         if( c == '\n' ) */
                /*         { */
                /*             Debug.Log( "hohohoho" ); */
                /*         } */
                /*     } */
                /* } */
            }


            //------------------------------------------------------------------------------
            public void ApplyAllUIText()
            {
                Text[] text_components = Resources.FindObjectsOfTypeAll<Text>();
                for (var i = 0; i < text_components.Length; ++i)
                {
                    InterpretText(ref text_components[i]);
                }
                /* foreach (var t in text_components) { */
                /*     InterpretText( ref t ); */
                /* } */
            }

            //------------------------------------------------------------------------------
            public void SetTagProcessor(ITagProcessor processor)
            {
                tagProcessor = processor;
            }

            //------------------------------------------------------------------------------
            public string GetText(string key)
            {
                string str = getTextRaw(key);
                if (tagProcessor != null)
                {
                    str = tagProcessor.Process(str);
                }
                return str;
            }

            //------------------------------------------------------------------------------
            public void InterpretText(ref Text t)
            {
                if (t.text.StartsWith("$$"))
                {
                    string key = t.text.Substring(2);
                    t.text = GetText(key);
                }
            }

            //------------------------------------------------------------------------------
            private string getTextRaw(string key)
            {
                switch (Config.Language)
                {
                    //case SystemLanguage.Japanese:
                    //    return texts[key].jp;
                    //    break;
                    //case SystemLanguage.Korean:
                    //    return texts[key].kor;
                    //    break;
                    //case SystemLanguage.ChineseSimplified:
                    //    return texts[key].sim;
                    //    break;
                    //case SystemLanguage.ChineseTraditional:
                    //    return texts[key].tra;
                    //    break;
                    case SystemLanguage.Vietnamese:
                        return texts[key].vie;
                        break;

                }
                /* if( Config.Language == SystemLanguage.Japanese ) */
                /* { */
                /*     return texts[key].jp; */
                /* } */
                /* else if( Config.Language == SystemLanguage.Korean ) */
                /* { */
                /*     return texts[key].kor; */
                /* } */
                /* else */
                /* { */
                /*     return texts[key].eng; */
                /* } */
                return texts[key].vie;
            }
        }

    }
}
