using System;
using System.Collections.Generic;
using System.Text;

namespace Elves.BattleSequencer {
    public static class Events {
        public static UVEvent UserTookNoDamage(UVUser user) => new TaglineEvent(Strings.UserTookNoDamage,user);
        public static UVEvent UserCannotHeal(UVUser user) => new TaglineEvent(Strings.UserTookNoDamage,user);
        public static UVEvent UserDamaged(UVUser user,int amount) => new TaglineEvent(Strings.UserDamaged,user,amount);
        public static UVEvent UserHealed(UVUser user,int amount) => new TaglineEvent(Strings.UserHealed,user,amount);
        public static UVEvent UserDidNothing(UVUser user) => new TaglineEvent(Strings.UserDidNothing,user);
        public static UVEvent UserHasDied(UVUser user) => new TaglineEvent(Strings.UserHasDied,user);
        public static UVEvent UserHello(UVUser user) => new TaglineEvent(Strings.UserHello,user);
        public static UVEvent UserGoodbye(UVUser user) => new TaglineEvent(Strings.UserGoodbye,user);
        public static UVEvent UserDisabled(UVUser user) => new TaglineEvent(Strings.UserDisabled,user);
        public static UVEvent NobodyIsHere() => new TaglineEvent(Strings.NobodyIsHere);
    }
}
