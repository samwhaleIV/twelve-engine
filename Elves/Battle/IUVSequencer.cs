using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elves.Battle {
    public interface IUVSequencer {
        public Task Tag(string text);
        public Task Speech(string text);
        public Task<int> GetOption(string[] options);
    }
}
