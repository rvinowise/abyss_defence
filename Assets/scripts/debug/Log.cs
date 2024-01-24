#define TRACE

using System;
using System.Diagnostics;


namespace rvinowise.debug {

public static class Log {

    public static TraceSource trace_source = new TraceSource("trace_source");

    static Log() {
        AppDomain.CurrentDomain.ProcessExit += StaticClass_Dtor;
        
        trace_source.Listeners.Add(
            new TextWriterTraceListener("TextWriterOutput.log") {
                Filter = new EventTypeFilter(SourceLevels.All)
            }
        );
        trace_source.Switch.Level = SourceLevels.All;
        //trace_source.Close();
        //trace_source.TraceInformation("Test message.");
        //trace_source.Flush();
        info("#########################################################");
    }

    static void StaticClass_Dtor(object sender, EventArgs e) {
        trace_source.Close();
    }

    public static void info(string text) {
        //trace_source.TraceInformation(text);
        trace_source.TraceEvent(TraceEventType.Information, 1, text);
        //trace_source.TraceEvent(TraceEventType.Warning, 1, text);
        //trace_source.TraceEvent(TraceEventType.Error, 1, text);
        trace_source.Flush();
    }

}
}