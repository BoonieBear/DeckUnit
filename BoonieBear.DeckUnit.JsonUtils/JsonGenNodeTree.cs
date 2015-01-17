namespace BoonieBear.DeckUnit.JsonUtils
{
    public class JsonGenNodeTree
    {
        #region Fields

        const string ARRAY = "[{0}]";
        const string NULL_TEXT = "<null>";
        const string OBJECT = "[{0}]";
        const string PROPERTY = "{0}";

        #endregion Fields

        #region Methods
        /*
        public  static NodeWriteLineic FromJson(string json)
        {
            var jobj = JObject.Parse(json);
            return FromJToken(jobj);
        }

        static NodeWriteLineic FromJToken(JToken jtoken)
        {
            if (jtoken is JValue)
            {
                var jvalue = (JValue)jtoken;
                var value = (jvalue.Value ?? NULL_TEXT).ToString();
                return new NodeWriteLineic(value, jvalue.Type.ToString(), null, TODO);
            }
            else if (jtoken is JContainer)
            {
                var jcontainer = (JContainer)jtoken;
                var children = jcontainer.Children().Select(FromJToken);
                string header;

                if (jcontainer is JProperty)
                {
                    header = String.Format(PROPERTY, ((JProperty)jcontainer).Name);
                    
                }
                else if (jcontainer is JArray)
                {
                    header = String.Format(ARRAY, children.Count());
                    
                }
                else if (jcontainer is JObject)
                {
                    header = String.Format(OBJECT, children.Count());
                    
                }
                else
                    throw new Exception("不支持的JContainer类型");

                return new NodeWriteLineic(header, NULL_TEXT, children, TODO);
            }
            else
            {
                throw new Exception("不支持的JToken类型");
            }
        }
        */
        #endregion Methods
    }
}
