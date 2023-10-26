using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapTopCam.MecroScirpt
{
    abstract class MecroMgr
    {
        public abstract bool AutoRun(ref AutoMecro _mecro);

        protected string GetFullPath()
        {
            return Directory.GetCurrentDirectory() + "\\" + "MecroImg\\";
        }

        protected bool resultfunc(int _succ)
        {
            if (_succ == 1)
                return false;
            else if (_succ == 2)
                return false;
            else
                return true;
        }

        
    }
}
