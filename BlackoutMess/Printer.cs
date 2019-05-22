using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Alba.CsConsoleFormat;
using static System.ConsoleColor;

namespace BlackoutMess
{
    public class Printer
    {
        public static void PrintChat(ChatData chatData)
        {
            var headerThickness = new LineThickness(LineWidth.None, LineWidth.None);
            var messageThickness = new LineThickness(LineWidth.None, LineWidth.None);


            var doc = new Document(
                new Span("Chat with ") {Color = Yellow}, chatData.UserName, "\n",
                new Span("New messages: ") {Color = Yellow}, chatData.CountNewMes,
                new Grid
                {
                    Color = Gray,
                    Columns = {GridLength.Auto, GridLength.Star(1)},
                    Children =
                    {
                        new Cell("Username ") {Align = Align.Right, Stroke = headerThickness},
                        new Cell("Message") {Align = Align.Center, Stroke = headerThickness},

                        chatData.Messages.Select(item => new object[]
                        {
                            new Cell(item.Name + ":")
                                {Align = Align.Center, Stroke = messageThickness, Color = Red},
                            new Cell(" " + item.MessageText) {Stroke = LineThickness.None},
                        })
                    }
                }
            );

            Console.Clear();
            ConsoleRenderer.RenderDocument(doc);
        }

        public static void PrintAvailableChats(List<ChatData> chats)
        {
            var headerThickness = new LineThickness(LineWidth.None, LineWidth.None);
            var messageThickness = new LineThickness(LineWidth.None, LineWidth.None);


            var doc = new Document(
                new Grid
                {
                    Color = Gray,
                    Columns = {GridLength.Auto, GridLength.Star(1)},
                    Children =
                    {
                        new Cell("ID ") {Align = Align.Right, Stroke = headerThickness},
                        new Cell("Username") {Align = Align.Left, Stroke = headerThickness},

                        chats.Select(item => new object[]
                        {
                            new Cell(item.IdUser)
                                {Align = Align.Right, Stroke = messageThickness, Color = Red},
                            new Cell(" " + item.UserName) {Stroke = LineThickness.None},
                        })
                    }
                }
            );

            Console.Clear();
            ConsoleRenderer.RenderDocument(doc);
        }

        public static void PrintSettings(List<Setting> settings)
        {
            var headerThickness = new LineThickness(LineWidth.None, LineWidth.None);
            var messageThickness = new LineThickness(LineWidth.None, LineWidth.None);


            var doc = new Document(
                new Grid
                {
                    Color = ConsoleColor.Gray,
                    Columns = {GridLength.Auto, GridLength.Auto, GridLength.Star(1)},
                    Children =
                    {
                        new Cell("Name ") {Align = Align.Center, Stroke = headerThickness, Color = ConsoleColor.Blue},
                        new Cell(" ") {Stroke = headerThickness},
                        new Cell(" State") {Align = Align.Left, Stroke = headerThickness},

                        settings.Select(item => new object[]
                        {
                            new Cell(item.Name)
                            {
                                Align = Align.Center, Stroke = messageThickness,
                                Color = ConsoleColor.Blue
                            },
                            new Cell(" =")
                            {
                                Align = Align.Center, Stroke = messageThickness,
                                Color = ConsoleColor.Blue
                            },
                            new Cell(" " + item.State) {Stroke = LineThickness.None},
                        })
                    }
                }
            );

            Console.Clear();
            ConsoleRenderer.RenderDocument(doc);
        }

        public static void PrintHelp()
        {
            var headerThickness = new LineThickness(LineWidth.None, LineWidth.None);
            var messageThickness = new LineThickness(LineWidth.None, LineWidth.None);

            Tuple<string, string>[] items =
            {
                new Tuple<string, string>("/help, /h", "shows help"),
                new Tuple<string, string>("/open chat_ID, /o chat_ID", "opens chat with specified ID"),
                new Tuple<string, string>("/chats, /c", "shows chats list"),
                new Tuple<string, string>("/redraw, /r", "updates current view"),
                new Tuple<string, string>("/add username, /a username", "adds chat with user"),
                new Tuple<string, string>("/exit, /e, /quit, /q", "closes app"),
                new Tuple<string, string>("any other text", "sends message to chat")
            };

            var doc = new Document(
                new Grid
                {
                    Color = Gray,
                    Columns = {GridLength.Auto, GridLength.Star(1)},
                    Children =
                    {
                        new Cell("command ") {Align = Align.Center, Stroke = headerThickness, Color = ConsoleColor.Blue},
                        new Cell("description") {Align = Align.Left, Stroke = headerThickness},

                        items.Select(item => new object[]
                        {
                            new Cell(item.Item1 + " ")
                            {
                                Align = Align.Left, Stroke = messageThickness
                            },
                            new Cell(item.Item2) {Stroke = LineThickness.None}
                        })
                    }
                }
            );
            
            ConsoleRenderer.RenderDocument(doc);
        }
    }
}