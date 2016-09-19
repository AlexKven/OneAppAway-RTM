using ExtraConstraints;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public abstract class WeakDelegateBase<TTarget, [DelegateConstraint] TDel> where TTarget : class
    {
        private Delegate StaticDelegate;
        private WeakReference<TTarget> Target;
        public WeakDelegateBase(TTarget target, TDel staticDelegate)
        {
            StaticDelegate = (Delegate)((object)staticDelegate);
            if (StaticDelegate.Target != null)
                throw new ArgumentException("The delegate must have a null target, otherwise a strong reference is held.", "staticDelegate");
            if (target != null)
                Target = new WeakReference<TTarget>(target);
        }
    }
}
