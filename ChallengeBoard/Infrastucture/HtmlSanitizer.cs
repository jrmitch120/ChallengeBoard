/** 
Copyright (c) 2009 Open Lab, http://www.open-lab.com/ 
Permission is hereby granted, free of charge, to any person obtaining 
a copy of this software and associated documentation files (the 
"Software"), to deal in the Software without restriction, including 
without limitation the rights to use, copy, modify, merge, publish, 
distribute, sublicense, and/or sell copies of the Software, and to 
permit persons to whom the Software is furnished to do so, subject to 
the following conditions: 
 
The above copyright notice and this permission notice shall be 
included in all copies or substantial portions of the Software. 
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace ChallengeBoard.Infrastucture
{

    public class HtmlSanitizer
    {
        public static Regex ForbiddenTags = new Regex("^(script|object|embed|link|style|form|input)$");
        public static Regex AllowedTags = new Regex("^(b|p|i|s|a|img|table|thead|tbody|tfoot|tr|th|td|dd|dl|dt|em|h1|h2|h3|h4|h5|h6|li|ul|ol|span|div|strike|strong|" +
                "sub|sup|pre|del|code|blockquote|strike|kbd|br|hr|area|map|object|embed|param|link|form|small|big)$");

        private static readonly Regex CommentPattern = new Regex("<!--.*");  // <!--.........> 
        private static readonly Regex TagStartPattern = new Regex("<(?i)(\\w+\\b)\\s*(.*)/?>$");  // <tag ....props.....> 
        private static readonly Regex TagClosePattern = new Regex("</(?i)(\\w+\\b)\\s*>$");  // </tag .........> 

        private static readonly Regex StandAloneTags = new Regex("^(img|br|hr)$");
        private static readonly Regex SelfClosed = new Regex("<.+/>");

        private static readonly Regex AttributesPattern = new Regex("(\\w*)\\s*=\\s*\"([^\"]*)\"");  // prop="...." 
        private static readonly Regex StylePattern = new Regex("([^\\s^:]+)\\s*:\\s*([^;]+);?");  // color:red; 

        private static readonly Regex UrlStylePattern = new Regex("(?i).*\\b\\s*url\\s*\\(['\"]([^)]*)['\"]\\)");  // url('....')" 

        public static Regex ForbiddenStylePattern = new Regex("(?:(expression|eval|javascript))\\s*\\(");  // expression(....)"   thanks to Ben Summer 


        /** 
         * This method should be used to test input. 
         * 
         * @param html 
         * @return true if the input is "valid" 
         */
        public static bool IsSanitized(String html)
        {
            return Sanitizer(html).IsValid;
        }




        /** 
         * Used to clean every html before to output it in any html page 
         * 
         * @param html 
         * @return sanitized html 
         */
        public static String Sanitize(String html)
        {
            return Sanitizer(html).Html;
        }

        /** 
         * Used to get the text,  tags removed or encoded 
         * 
         * @param html 
         * @return sanitized text 
         */
        public static String GetText(String html)
        {
            return Sanitizer(html).Text;
        }


        /** 
         * This is the main method of sanitizing. It will be used both for validation and cleaning 
         * 
         * @param html 
         * @return a SanitizeResult object 
         */
        public static SanitizeResult Sanitizer(String html)
        {
            return Sanitizer(html, AllowedTags, ForbiddenTags);
        }

        public static SanitizeResult Sanitizer(String html, Regex allowedTags, Regex forbiddenTags)
        {
            var ret = new SanitizeResult();
            var openTags = new Stack<string>();

            if (String.IsNullOrEmpty(html))
                return ret;

            IEnumerable<string> tokens = Tokenize(html);

            // -------------------   LOOP for every token -------------------------- 
            foreach (var t1 in tokens)
            {
                String token = t1;
                bool isAcceptedToken = false;

                Match startMatcher = TagStartPattern.Match(token);
                Match endMatcher = TagClosePattern.Match(token);

                //--------------------------------------------------------------------------------  COMMENT    <!-- ......... --> 
                if (CommentPattern.Match(token).Success)
                {
                    ret.Val = ret.Val + token + (token.EndsWith("-->") ? "" : "-->");
                    ret.InvalidTags.Add(token + (token.EndsWith("-->") ? "" : "-->"));
                    continue;

                    //--------------------------------------------------------------------------------  OPEN TAG    <tag .........> 
                }
                if (startMatcher.Success)
                {

                    //tag name extraction 
                    String tag = startMatcher.Groups[1].Value.ToLower();

                    //-----------------------------------------------------  FORBIDDEN TAG   <script .........> 
                    if (forbiddenTags.Match(tag).Success)
                    {
                        ret.InvalidTags.Add("<" + tag + ">");
                        continue;

                        // --------------------------------------------------  WELL KNOWN TAG 
                    }
                    if (allowedTags.Match(tag).Success)
                    {

                        String cleanToken = "<" + tag;
                        String tokenBody = startMatcher.Groups[2].Value;

                        //first test table consistency 
                        //table tbody tfoot thead th tr td 
                        if ("thead".Equals(tag) || "tbody".Equals(tag) || "tfoot".Equals(tag) || "tr".Equals(tag))
                        {
                            if (!openTags.Select(t => t == "table").Any())
                            {
                                ret.InvalidTags.Add("<" + tag + ">");
                                continue;
                            }
                        }
                        else if ("td".Equals(tag) || "th".Equals(tag))
                        {
                            if (openTags.Count(t => t == "tr") <= 0)
                            {
                                ret.InvalidTags.Add("<" + tag + ">");
                                continue;
                            }
                        }

                        // then test properties 
                        //Match attributes = attributesPattern.Match(tokenBody);
                        var attributes = AttributesPattern.Matches(tokenBody);

                        bool foundUrl = false; // URL flag

                        foreach (Match attribute in attributes)
                            //while (attributes.find())
                        {
                            String attr = attribute.Groups[1].Value.ToLower();
                            String val = attribute.Groups[2].Value;

                            // we will accept href in case of <A> 
                            if ("a".Equals(tag) && "href".Equals(attr))
                            {    // <a href="......">


                                try
                                {
                                    var url = new Uri(val);

                                    if (url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps || url.Scheme == Uri.UriSchemeMailto)
                                    {
                                        foundUrl = true;
                                    }
                                    else
                                    {
                                        ret.InvalidTags.Add(attr + " " + val);
                                        val = "";
                                    }
                                }
                                catch
                                {
                                    ret.InvalidTags.Add(attr + " " + val);
                                    val = "";
                                }
                            }
                            else if ((tag == "img" || tag == "embed") && "src".Equals(attr))
                            { // <img src="......"> 
                                try
                                {
                                    var url = new Uri(val);

                                    if (url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps)
                                    {
                                        foundUrl = true;
                                    }
                                    else
                                    {
                                        ret.InvalidTags.Add(attr + " " + val);
                                        val = "";
                                    }
                                }
                                catch
                                {
                                    ret.InvalidTags.Add(attr + " " + val);
                                    val = "";
                                }

                            }
                            else if ("href".Equals(attr) || "src".Equals(attr))
                            { // <tag src/href="......">   skipped 
                                ret.InvalidTags.Add(tag + " " + attr + " " + val);
                                continue;

                            }
                            else if (attr == "width" || attr == "height")
                            { // <tag width/height="......">
                                var r = new Regex("\\d+%|\\d+$");
                                if (!r.Match(val.ToLower()).Success)
                                { // test numeric values 
                                    ret.InvalidTags.Add(tag + " " + attr + " " + val);
                                    continue;
                                }

                            }
                            else if ("style".Equals(attr))
                            { // <tag style="......"> 

                                // then test properties 
                                var styles = StylePattern.Matches(val);
                                String cleanStyle = "";

                                foreach (Match style in styles)
                                    //while (styles.find())
                                {
                                    String styleName = style.Groups[1].Value.ToLower();
                                    String styleValue = style.Groups[2].Value;

                                    // suppress invalid styles values 
                                    if (ForbiddenStylePattern.Match(styleValue).Success)
                                    {
                                        ret.InvalidTags.Add(tag + " " + attr + " " + styleValue);
                                        continue;
                                    }

                                    // check if valid url 
                                    Match urlStyleMatcher = UrlStylePattern.Match(styleValue);
                                    if (urlStyleMatcher.Success)
                                    {
                                        try
                                        {
                                            String url = urlStyleMatcher.Groups[1].Value;
                                            var uri = new Uri(url);

                                            if (!(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                                            {
                                                ret.InvalidTags.Add(tag + " " + attr + " " + styleValue);
                                                continue;
                                            }
                                        }
                                        catch
                                        {
                                            ret.InvalidTags.Add(tag + " " + attr + " " + styleValue);
                                            continue;
                                        }

                                    }

                                    cleanStyle = cleanStyle + styleName + ":" + Encode(styleValue) + ";";

                                }
                                val = cleanStyle;

                            }
                            else if (attr.StartsWith("on"))
                            {  // skip all javascript events 
                                ret.InvalidTags.Add(tag + " " + attr + " " + val);
                                continue;

                            }
                            else
                            {  // by default encode all properies 
                                val = Encode(val);
                            }

                            cleanToken = cleanToken + " " + attr + "=\"" + val + "\"";
                        }
                        if (SelfClosed.Match(token).Success)
                            cleanToken = cleanToken + "/>";
                        else
                            cleanToken = cleanToken + ">";

                        isAcceptedToken = true;

                        // for <img> and <a>
                        if ((tag == "a" || tag == "img" || tag == "embed") && !foundUrl)
                        {
                            isAcceptedToken = false;
                            cleanToken = "";
                        }

                        token = cleanToken;

                        // push the tag if require closure and it is accepted (otherwise is encoded) 
                        if (isAcceptedToken && !(StandAloneTags.Match(tag).Success || SelfClosed.Match(token).Success))
                            openTags.Push(tag);

                        // --------------------------------------------------------------------------------  UNKNOWN TAG 
                    }
                    else
                    {
                        ret.InvalidTags.Add(token);
                        ret.Val = ret.Val + token;
                        continue;

                    }

                    // --------------------------------------------------------------------------------  CLOSE TAG </tag> 
                }
                else if (endMatcher.Success)
                {
                    String tag = endMatcher.Groups[1].Value.ToLower();

                    //is self closing 
                    if (SelfClosed.Match(tag).Success)
                    {
                        ret.InvalidTags.Add(token);
                        continue;
                    }
                    if (forbiddenTags.Match(tag).Success)
                    {
                        ret.InvalidTags.Add("/" + tag);
                        continue;
                    }
                    if (!allowedTags.Match(tag).Success)
                    {
                        ret.InvalidTags.Add(token);
                        ret.Val = ret.Val + token;
                        continue;
                    }
                    String cleanToken = "";

                    // check tag position in the stack 

                    int pos = -1;
                    bool found = false;

                    foreach (var item in openTags)
                    {
                        pos++;
                        if (item == tag)
                        {
                            found = true;
                            break;
                        }
                    }

                    // if found on top ok 
                    if (found)
                    {
                        for (int k = 0; k <= pos; k++)
                        {
                            //pop all elements before tag and close it 
                            String poppedTag = openTags.Pop();
                            cleanToken = cleanToken + "</" + poppedTag + ">";
                            isAcceptedToken = true;
                        }
                    }

                    token = cleanToken;
                }

                ret.Val = ret.Val + token;

                if (isAcceptedToken)
                {
                    ret.Html = ret.Html + token;
                    //ret.text = ret.text + " "; 
                }
                else
                {
                    String sanToken = HtmlEncodeApexesAndTags(token);
                    ret.Html = ret.Html + sanToken;
                    ret.Text = ret.Text + HtmlEncodeApexesAndTags(RemoveLineFeed(token));
                }
            }

            // must close remaining tags 
            while (openTags.Any())
            {
                //pop all elements before tag and close it 
                String poppedTag = openTags.Pop();
                ret.Html = ret.Html + "</" + poppedTag + ">";
                ret.Val = ret.Val + "</" + poppedTag + ">";
            }

            //set boolean value 
            ret.IsValid = ret.InvalidTags.Count == 0;

            return ret;
        }

        /** 
         * Splits html tag and tag content <......>. 
         * 
         * @param html 
         * @return a list of token 
         */
        private static IEnumerable<string> Tokenize(String html)
        {
            //ArrayList tokens = new ArrayList();
            var tokens = new List<string>();
            int pos = 0;
            String token = "";
            int len = html.Length;
            while (pos < len)
            {
                char c = html[pos];

                // BBB String ahead = html.Substring(pos, pos > len - 4 ? len : pos + 4);
                String ahead = html.Substring(pos, pos > len - 4 ? len - pos : 4);

                //a comment is starting 
                if ("<!--".Equals(ahead))
                {
                    //store the current token 
                    if (token.Length > 0)
                        tokens.Add(token);

                    //clear the token 
                    token = "";

                    // serch the end of <......> 
                    int end = MoveToMarkerEnd(pos, "-->", html);

                    // BBB tokens.Add(html.Substring(pos, end));
                    tokens.Add(html.Substring(pos, end - pos));
                    pos = end;


                    // a new "<" token is starting 
                }
                else if ('<' == c)
                {

                    //store the current token 
                    if (token.Length > 0)
                        tokens.Add(token);

                    //clear the token 
                    token = "";

                    // serch the end of <......> 
                    int end = MoveToMarkerEnd(pos, ">", html);
                    // BBB tokens.Add(html.Substring(pos, end));
                    tokens.Add(html.Substring(pos, end - pos));
                    pos = end;

                }
                else
                {
                    token = token + c;
                    pos++;
                }

            }

            //store the last token 
            if (token.Length > 0)
                tokens.Add(token);

            return tokens;
        }


        private static int MoveToMarkerEnd(int pos, String marker, String s)
        {
            int i = s.IndexOf(marker, pos, StringComparison.Ordinal);
            if (i > -1)
                pos = i + marker.Length;
            else
                pos = s.Length;
            return pos;
        }

        /** 
         * Contains the sanitizing results. 
         * html is the sanitized html encoded  ready to be printed. Unaccepted tags are encode, text inside tag is always encoded   MUST BE USED WHEN PRINTING HTML 
         * text is the text inside valid tags. Contains invalid tags encoded                                                        SHOULD BE USED TO PRINT EXCERPTS 
         * val  is the html source cleaned from unaccepted tags. It is not encoded:                                                 SHOULD BE USED IN SAVE ACTIONS 
         * isValid is true when every tag is accepted without forcing encoding 
         * invalidTags is the list of encoded-killed tags 
         */
        public class SanitizeResult
        {
            public String Html = "";
            public String Text = "";
            public String Val = "";
            public bool IsValid = true;
            public List<String> InvalidTags = new List<string>();
        }

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public static String Encode(String s)
        {
            return ConvertLineFeedToBr(HtmlEncodeApexesAndTags(s ?? ""));
        }

        public static String HtmlEncodeApexesAndTags(String source)
        {
            return HtmlEncodeTag(HtmlEncodeApexes(source));
        }

        public static String HtmlEncodeApexes(String source)
        {
            if (source != null)
            {
                String result = ReplaceAllNoRegex(source, new[] { "\"", "'" }, new[] { "&quot;", "&#39;" });
                return result;
            }
            
            return null;
        }

        public static String HtmlEncodeTag(String source)
        {
            if (source != null)
            {
                String result = ReplaceAllNoRegex(source, new[] { "<", ">" }, new[] { "&lt;", "&gt;" });
                return result;
            }
            
            return null;
        }


        public static String ConvertLineFeedToBr(String text)
        {
            if (text != null)
                return ReplaceAllNoRegex(text, new [] { "\n", "\f", "\r" }, new [] { "<br>", "<br>", " " });
            
            return null;
        }

        public static String RemoveLineFeed(String text)
        {
            if (text != null)
                return ReplaceAllNoRegex(text, new[] { "\n", "\f", "\r" }, new [] { " ", " ", " " });
            
            return null;
        }


        public static String ReplaceAllNoRegex(String source, String[] searches, String[] replaces)
        {
            int k;
            String tmp = source;
            for (k = 0; k < searches.Length; k++)
                tmp = ReplaceAllNoRegex(tmp, searches[k], replaces[k]);
            return tmp;
        }

        public static String ReplaceAllNoRegex(String source, String search, String replace)
        {
            var buffer = new StringBuilder();
            if (source != null)
            {
                if (search.Length == 0)
                    return source;
                int oldPos, pos;
                for (oldPos = 0, pos = source.IndexOf(search, oldPos, StringComparison.Ordinal); pos != -1; oldPos = pos + search.Length, pos = source.IndexOf(search, oldPos, StringComparison.Ordinal))
                {
                    buffer.Append(source.Substring(oldPos, pos - oldPos));
                    buffer.Append(replace);
                }
                if (oldPos < source.Length)
                    buffer.Append(source.Substring(oldPos));
            }
            return buffer.ToString();
        }
    }
}