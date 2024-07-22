using System;
using System.ComponentModel;
using System.Diagnostics;

namespace ti_Lyricstudio.My
{
    internal static partial class MyProject
    {
        internal partial class MyForms
        {

            [EditorBrowsable(EditorBrowsableState.Never)]
            public AddMultipleLineWindow m_AddMultipleLineWindow;

            public AddMultipleLineWindow AddMultipleLineWindow
            {
                [DebuggerHidden]
                get
                {
                    m_AddMultipleLineWindow = Create__Instance__(m_AddMultipleLineWindow);
                    return m_AddMultipleLineWindow;
                }
                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_AddMultipleLineWindow))
                        return;
                    if (value is not null)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_AddMultipleLineWindow);
                }
            }


            [EditorBrowsable(EditorBrowsableState.Never)]
            public DebugWindow m_DebugWindow;

            public DebugWindow DebugWindow
            {
                [DebuggerHidden]
                get
                {
                    m_DebugWindow = Create__Instance__(m_DebugWindow);
                    return m_DebugWindow;
                }
                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_DebugWindow))
                        return;
                    if (value is not null)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_DebugWindow);
                }
            }


            [EditorBrowsable(EditorBrowsableState.Never)]
            public MainWindow m_MainWindow;

            public MainWindow MainWindow
            {
                [DebuggerHidden]
                get
                {
                    m_MainWindow = Create__Instance__(m_MainWindow);
                    return m_MainWindow;
                }
                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_MainWindow))
                        return;
                    if (value is not null)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_MainWindow);
                }
            }

        }


    }
}