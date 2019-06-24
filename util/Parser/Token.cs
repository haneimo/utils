/*
  csc /t:library .\Token.cs 
*/
namespace Parser
{

    public class Token
    {

        private string _body;
        public string Body { get { return _body; } }
        private string _typeName;
        public string TypeName { get { return _typeName; } }

        private static Token _emptyToken = new Token("", "");
        public static Token EMPTY_TOKEN { get { return _emptyToken; } }

        public Token(string typeName, string body)
        {
            this._typeName = typeName;
            this._body = body;
        }

        public bool IsEmpty()
        {
            return (this._typeName == "" && this._body == "");
        }

        public override string ToString()
        {
            return "TypeName:" + _typeName + ", Body:" + _body;
        }
    }
}