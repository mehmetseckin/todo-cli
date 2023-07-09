// HACK_KEY2: drp070823 - in case task was "renamed" as well as "Moved", try to find match using createdDateTime only.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Graph;

namespace Todo.Core.Model
{
    /* REF:
     *       "@odata.etag": "W/\"rUIjU8NJN0aB7BwuPLdcMAAGkziqLA==\"",
      "importance": "normal",
      "isReminderOn": false,
      "status": "notStarted",
      "title": "TASKTITLE",
      "createdDateTime": "2022-05-05T22:25:58.7733259Z",
      "lastModifiedDateTime": "2023-02-11T00:11:54.5847607Z",
      "hasAttachments": false,
      "categories": [],
      "id": "AQMkADAwATEwYmM3LTU4NjktYTUxYS0wMAItMDAKAEYAAAOEN7IbTiy5RKcfAAfx3varBwCtQiNTw0k3RoHsHC48t1wwAAMql8UPAAAArUIjU8NJN0aB7BwuPLdcMAAGktwqlwAAAA==",
      "body": {
        "content": "TASKNOTES",
        "contentType": "text"
      },
      "checklistItems@odata.context": "https://graph.microsoft.com/v1.0/$metadata#users('EMAILUSERNAME%40hotmail.com')/todo/lists('AQMkADAwATEwYmM3LTU4NjktYTUxYS0wMAItMDAKAC4AAAOEN7IbTiy5RKcfAAfx3varAQCtQiNTw0k3RoHsHC48t1wwAAMql8UPAAAA')/tasks('AQMkADAwATEwYmM3LTU4NjktYTUxYS0wMAItMDAKAEYAAAOEN7IbTiy5RKcfAAfx3varBwCtQiNTw0k3RoHsHC48t1wwAAMql8UPAAAArUIjU8NJN0aB7BwuPLdcMAAGktwqlwAAAA%3D%3D')/checklistItems",
      "checklistItems": [
        {
          "displayName": "SUBTASKTITLE",
          "createdDateTime": "2022-05-05T22:26:07.7199032Z",
          "checkedDateTime": "2022-05-07T14:47:15.5658684Z",
          "isChecked": true,
          "id": "32da999e-acbc-4f1d-b7c5-a5c9dcbc284c"
        },
        {
          "displayName": "SUBTASKTITLE",
          "createdDateTime": "2022-05-06T23:31:20.7224936Z",
          "isChecked": false,
          "id": "2b66055a-033b-4578-a826-26aefc7426fc"
        },
      ]
    },
    */
    public class TodoItem : IJsonOnDeserialized
    {
        public class Body
        {
            public string content { get; set; }
            public string contentType { get; set; }
        }

        public class ChecklistItem
        {
            public string displayName { get; set; }
            public DateTime createdDateTime { get; set; }
            public DateTime checkedDateTime { get; set; }
            public bool isChecked { get; set; }
            public string id { get; set; }
        }

        public string status {  get; set; }
        public string title { get; set; }
        public DateTime createdDateTime { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public bool hasAttachments { get; set; }
        public string id { get; set; }

        public Body body { get; set; }

        public List<ChecklistItem> checklistItems { get; set; }

        [JsonIgnore]
        public string OriginalSerialized { get; set; }

        public override string ToString()
        {
            return title;
        }

        // TECH: invoked through IJsonOnDeserialized
        public void OnDeserialized()
        {
            Key = new TodoItemKey(this);
            Key2 = new TodoItemKey2(this);
        }

        #region Exported

        public TodoItemKey Key { get; private set; }

        // HACK_KEY2
        public TodoItemKey2 Key2 { get; private set; }

        public TodoList List { get; private set; }

        public FileInfo FileInfo { get; private set; }

        public void OnTodoItemExported(TodoList list, FileInfo fi)
        {
            if (FileInfo != null)
                throw new InvalidOperationException();
            List = list;
            FileInfo = fi;
        }

        #endregion
    }
}
