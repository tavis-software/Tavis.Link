// Adapted from Jorn Wildt's code https://github.com/JornWildt/Ramone

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Security.Application;
using Encoder = Microsoft.Security.Application.Encoder;

namespace Tavis
{
    public class LinkHeaderParser
    {


        public Uri BaseUrl { get; protected set; }


    public IList<Link> Parse(Uri baseUrl, string linkHeader)
    {
      BaseUrl = baseUrl;
      InputString = linkHeader;
      InputPos = 0;

      IList<Link> links = new List<Link>();

      while (true)
      {
        try
        {
          GetNextToken();

          if (NextToken.Type == TokenType.Url)
            links.Add(ParseLink());
          else if (NextToken.Type == TokenType.EOF)
            break;
          else
            Error(string.Format("Unexpected token '{0}' (expected URL)", NextToken.Type));

          if (NextToken.Type == TokenType.Comma)
            continue;
          else if (NextToken.Type == TokenType.EOF)
            break;
          else
            Error(string.Format("Unexpected token '{0}' (expected comma)", NextToken.Type));
        }
        catch (FormatException)
        {
          while (NextToken.Type != TokenType.Comma && NextToken.Type != TokenType.EOF)
          {
            try
            {
              GetNextToken();
            }
            catch (FormatException)
            {
            }
          }
        }
      }

      return links;
    }


    #region Parser

    protected Link ParseLink()
    {
//      Condition.Requires(NextToken.Type, "CurrentToken.Type").IsEqualTo(TokenType.Url);

      string url = NextToken.Value;
      string rel = null;
      string title = null;
      string title_s = null;
      string type = null;

      GetNextToken();

      while (NextToken.Type == TokenType.Semicolon)
      {
        try
        {
          GetNextToken();
          bool isExtended;
          KeyValuePair<string, string> p = ParseParameter(out isExtended);

          if (p.Key == "rel" && rel == null)
            rel = p.Value;
          else if (p.Key == "title" && title == null && !isExtended)
            title = p.Value;
          else if (p.Key == "title" && title_s == null && isExtended)
            title_s = p.Value;
          else if (p.Key == "type" && type == null)
            type = p.Value;
        }
        catch (FormatException)
        {
          while (NextToken.Type != TokenType.Semicolon && NextToken.Type != TokenType.Comma && NextToken.Type != TokenType.EOF)
          {
            try
            {
              GetNextToken();
            }
            catch (FormatException)
            {
            }
          }
        }
      }

        Link link = new Link() {
            Target = new Uri(BaseUrl, url),
            Relation = rel, 
            Type = MediaTypeHeaderValue.Parse(type), 
            Title=  title_s ?? title
    };
      return link;
    }


    protected KeyValuePair<string, string> ParseParameter(out bool isExtended)
    {
      if (NextToken.Type != TokenType.Identifier && NextToken.Type != TokenType.ExtendedIdentifier)
        Error(string.Format("Unexpected token '{0}' (expected an identifier)", NextToken.Type));
      string id = NextToken.Value;
      isExtended = (NextToken.Type == TokenType.ExtendedIdentifier);
      GetNextToken();

      if (NextToken.Type != TokenType.Assignment)
        Error(string.Format("Unexpected token '{0}' (expected an assignment)", NextToken.Type));

      if (id == "rel")
      {
        GetNextStringOrRelType();
      }
      else
      {
        GetNextToken();
      }

      if (NextToken.Type != TokenType.String)
        Error(string.Format("Unexpected token '{0}' (expected an string)", NextToken.Type));
      string value = NextToken.Value;
      if (isExtended)
        value = HeaderEncodingParser.ParseExtendedHeader(value);
      GetNextToken();

      return new KeyValuePair<string, string>(id, value);
    }


    #endregion


    #region Token scanner

    protected Token NextToken { get; set; }

    protected enum TokenType { Url, Semicolon, Comma, Assignment, Identifier, ExtendedIdentifier, String, EOF }

    protected class Token
    {
      public TokenType Type { get; set; }
      public string Value { get; set; }
    }

    protected string InputString { get; set; }
    protected int InputPos { get; set; }


    protected void GetNextToken()
    {
      NextToken = ReadToken();
    }


    protected void GetNextStringOrRelType()
    {
      NextToken = ReadNextStringOrRelType();
    }


    protected Token ReadToken()
    {
      while (true)
      {
        char? c = ReadNextChar();

        if (c == null)
          return new Token { Type = TokenType.EOF };

        if (c == ';')
          return new Token { Type = TokenType.Semicolon };

        if (c == ',')
          return new Token { Type = TokenType.Comma };

        if (c == '=')
          return new Token { Type = TokenType.Assignment };

        if (c == '"')
          return new Token { Type = TokenType.String, Value = ReadString() };

        if (c == '<')
          return new Token { Type = TokenType.Url, Value = ReadUrl() };

        if (Char.IsWhiteSpace(c.Value))
          continue;

        if (Char.IsLetter(c.Value))
          return ReadIdentifier(c.Value);

        Error(string.Format("Unrecognized character '{0}'", c));
      }
    }


    protected Token ReadNextStringOrRelType()
    {
      while (true)
      {
        char? c = ReadNextChar();

        if (c == null)
          return new Token { Type = TokenType.EOF };

        if (c == '"')
          return new Token { Type = TokenType.String, Value = ReadString() };

        if (Char.IsLetter(c.Value))
          return new Token { Type = TokenType.String, Value = ReadRelType(c.Value) };

        Error(string.Format("Unrecognized character '{0}' for string or rel-type", c));
      }
    }


    protected string ReadString()
    {
      string result = "";

      while (true)
      {
        char? c = ReadNextChar();
        if (c == null)
          break;
        if (c == '"')
          break;
        result += c;
      }

      return result;
    }


    protected string ReadUrl()
    {
      string result = "";

      while (true)
      {
        char? c = ReadNextChar();
        if (c == null)
          break;
        if (c == '>')
          break;
        result += c;
      }

      return result;
    }


    protected Token ReadIdentifier(char c)
    {
      string id = "" + c;

      while (Char.IsLetterOrDigit(InputString[InputPos]))
      {
        id += InputString[InputPos++];
      }

      if (InputString[InputPos] == '*')
      {
        InputPos++;
        return new Token { Type = TokenType.ExtendedIdentifier, Value = id };
      }
      else
      {
        return new Token { Type = TokenType.Identifier, Value = id };
      }
    }


    protected string ReadRelType(char c)
    {
      string id = "" + c;

      while (Char.IsLetterOrDigit(InputString[InputPos]) || InputString[InputPos] == '.' || InputString[InputPos] == '-')
      {
        id += InputString[InputPos++];
      }

      return id;
    }


    protected char? ReadNextChar()
    {
      if (InputPos == InputString.Length)
        return null;
      return InputString[InputPos++];
    }

    #endregion


    protected void Error(string msg)
    {
      throw new FormatException(string.Format("Invalid HTTP Web Link. {0} in '{1}' (around pos {2}).", msg, InputString, InputPos));
    }
  }


    public class HeaderEncodingParser
    {
        public static string ParseExtendedHeader(string s)
        {
            HeaderEncodingParser parser = new HeaderEncodingParser();
            return parser.Parse(s);
        }


        public string Parse(string header)
        {
     //       Condition.Requires(header, "header").IsNotNull();
            string[] parts = header.Split(new char[] { '\'' }, 3);

            string charset = (parts.Length == 3 ? parts[0] : null);
            string language = (parts.Length == 3 ? parts[1] : null);
            string content = parts[parts.Length - 1];

            try {
                var enc = Encoding.GetEncoding(charset);
                return UriDecode(content, enc);
            }
            catch (Exception)
            {
                return content;
            }
        }


        // https://github.com/pvginkel/NHttp/blob/98f095afe18c66db71acd881c70ca91063ba7f48/NHttp/HttpUtil.cs




        public static string UriDecode(string value, Encoding encoding)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            byte[] result = new byte[value.Length];
            int length = 0;

            for (int i = 0; i < value.Length; i++)
            {
                if (
                    value[i] == '%' &&
                    i < value.Length - 2 &&
                    IsHex(value[i + 1]) &&
                    IsHex(value[i + 2])
                )
                {
                    result[length++] = (byte)(HexToInt(value[i + 1]) * 16 + HexToInt(value[i + 2]));

                    i += 2;
                }
                else if (value[i] == '+')
                {
                    result[length++] = (byte)' ';
                }
                else
                {
                    int c = value[i];

                    if (c > byte.MaxValue)
                        throw new InvalidOperationException("URI contained unexpected character");

                    result[length++] = (byte)c;
                }
            }

            return encoding.GetString(result, 0, length);
        }


        private static int HexToInt(char value)
        {
            switch (value)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return value - '0';

                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    return (value - 'a') + 10;

                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    return (value - 'A') + 10;

                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }


        private static bool IsHex(char value)
        {
            switch (value)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    return true;

                default:
                    return false;
            }
        }

    }




}
