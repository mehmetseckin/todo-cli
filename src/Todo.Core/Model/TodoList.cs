/* REF: JSONS /lists response
Content:
{
  "@odata.context": "https://graph.microsoft.com/v1.0/$metadata#users('EMAILUSERNAME%40hotmail.com')/todo/lists",
  "@odata.nextLink": "https://graph.microsoft.com/v1.0/me/todo/lists?$skip=50",
  "value": [
    {
      "@odata.etag": "W/\"rUIjU8NJN0aB7BwuPLdcMAAGqyAOCQ==\"",
      "displayName": "LISTNAME",
      "isOwner": true,
      "isShared": false,
      "wellknownListName": "defaultList",
      "id": "AQMkADAwATEwYmM3LTU4NjktYTUxYS0wMAItMDAKAC4AAAOEN7IbTiy5RKcfAAfx3varAQCtQiNTw0k3RoHsHC48t1wwAAACARIAAAA="
    },
    {
      "@odata.etag": "W/\"rUIjU8NJN0aB7BwuPLdcMAAGqyAODw==\"",
      "displayName": "LISTNAME",
      "isOwner": true,
      "isShared": false,
      "wellknownListName": "none",
      "id": "AQMkADAwATEwYmM3LTU4NjktYTUxYS0wMAItMDAKAC4AAAOEN7IbTiy5RKcfAAfx3varAQCtQiNTw0k3RoHsHC48t1wwAAU0APXtAAAA"
    },
  ]
}
*/

//fnord - does this only return first 50 lists? see nextLink

using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Core.Model
{
    public class TodoList
    {
        public string displayName { get; set; }

        public bool isOwner { get; set; }
        public bool isShared { get; set; }
        public string wellknownListName {
            get;
            set;
        }
        public string id { get; set; }
    }
}
