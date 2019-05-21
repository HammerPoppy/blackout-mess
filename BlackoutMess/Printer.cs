using System;
using System.Collections.Generic;
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
                        new Cell("ID") {Align = Align.Right, Stroke = headerThickness},
                        new Cell("Username") {Align = Align.Center, Stroke = headerThickness},

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
    }
}