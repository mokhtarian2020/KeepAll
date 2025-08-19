using Android.App;
using Android.Content;
using Android.OS;

namespace KeepAll.App;

[Activity(Label = "Save to KeepAll", NoHistory = true, Exported = true)]
[IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "text/plain")]
[IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "text/uri-list")]
public class ShareReceiverActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        var shared = Intent?.GetStringExtra(Intent.ExtraText) ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(shared))
        {
            // Enqueue into a simple pending list in Preferences; main app imports on next load
            var key = "pending_shared_items";
            var existing = Preferences.Get(key, "");
            var next = string.IsNullOrWhiteSpace(existing) ? shared : $"{existing}\n{shared}";
            Preferences.Set(key, next);
        }

        Finish();
    }
}
