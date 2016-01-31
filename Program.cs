using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using LeagueSharp;
using SharpDX;
using LeagueSharp.Common;

namespace StreamSharp
{
    class Program
    {
        private static Render.Sprite CursorAttack, CursorMove;
        public static float starttimex = 0;
        public static float starttimey = 0;
        public static Menu Option;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += onGameLoad;
        }

        #region onGameLoad
        private static void onGameLoad(EventArgs args)
        {
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnIssueOrder += GameObject_issueOrder;
            Game.OnWndProc += Game_OnWndProc;
            Drawing.OnDraw += OnDraw;

            Option = new Menu("StreamSharp", "Stream Sharp", true);
            Option.SubMenu("Keys").AddItem(new MenuItem("X", "LastHit").SetValue((new KeyBind("X".ToCharArray()[0], KeyBindType.Press))));
            Option.SubMenu("Keys").AddItem(new MenuItem("C", "Harras").SetValue((new KeyBind("C".ToCharArray()[0], KeyBindType.Press))));
            Option.SubMenu("Keys").AddItem(new MenuItem("V", "Clear").SetValue((new KeyBind("V".ToCharArray()[0], KeyBindType.Press))));
            Option.SubMenu("Keys").AddItem(new MenuItem("Space", "Combo").SetValue((new KeyBind(32, KeyBindType.Press))));
            Option.AddToMainMenu();

            CursorAttack = new Render.Sprite(Properties.Resources.Attack, new Vector2((Drawing.Width / 2) - 500, (Drawing.Height / 2) - 350));
            CursorAttack.Add(0);
            CursorAttack.Visible = false;
            CursorAttack.OnDraw();

            CursorMove = new Render.Sprite(Properties.Resources.normal, new Vector2((Drawing.Width / 2) - 500, (Drawing.Height / 2) - 350));
            CursorMove.Add(0);
            CursorMove.OnDraw();


        }
        #endregion

        static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!sender.Owner.IsMe)
                return;
            CursorMove.Position = Drawing.WorldToScreen(args.StartPosition);
        }

        static void GameObject_issueOrder(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (args.Order == GameObjectOrder.AttackUnit)
            {
                FakeCursorMove(Drawing.WorldToScreen(args.TargetPosition));
                CursorAttack.Visible = true;
                CursorMove.Visible = false;
            }
            if (args.Order == GameObjectOrder.MoveTo)
            {
                FakeCursorMove(Drawing.WorldToScreen(args.TargetPosition));

                CursorAttack.Visible = false;
                CursorMove.Visible = true;
            }
        }

        static void FakeCursorMove(Vector2 Endpos)
        {
            CursorMove.Position = Endpos;
            CursorAttack.Position = Endpos;
        }

        static void Game_OnWndProc(WndEventArgs args)
        {
            if (MenuGUI.IsChatOpen)
                return;

            if (args.Msg == (uint)WindowsMessages.WM_RBUTTONDOWN)
            {
                CursorMove.Position = Utils.GetCursorPos();
                CursorAttack.Position = Utils.GetCursorPos();
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var X = Option.Item("X").GetValue<KeyBind>().Active;
            var C = Option.Item("C").GetValue<KeyBind>().Active;
            var V = Option.Item("V").GetValue<KeyBind>().Active;
            var Space = Option.Item("Space").GetValue<KeyBind>().Active;
            if (!Space && !X && !C && !V)
            {
                CursorMove.Position = Utils.GetCursorPos();
                CursorAttack.Position = Utils.GetCursorPos();
            }
        }
    }
}
