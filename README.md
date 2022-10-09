# MailboxMauler

This script uses MailKit to connect to an IMAP inbox and move *ANY* message in the Inbox older than a certain date into the trash.

I wrote this to solve my grandmother's problem of having a mailbox which was misconfigured on her phone as POP3 and did not delete messages from the server upon retrieval. The mailbox according to IMAP / webmail was 99% full with 76,000 spam / junk emails dating back to 2013.

The bulk delete for most mailboxes is only 50-100 messages at a time.

## Caveats

This implementation has an admittedly sloppy loop which moves messages from the Inbox to the trash folder. It does not mark the message as deleted in the inbox, as these are two different actions according to IMAP. This will do a MOVE command if the server supports it or a COPY and mark as deleted from the source as a fallback. Beware that it does, however, expunge the inbox after the messsage moves are complete.

Your email provider's IMAP implementation may vary. In the mailbox I developed this against, I had to expunge the records from the inbox after moving them to reclaim space. Emails would go into the trash during the run of the script but after expunging, the trash did seem to empty itself instead of retaining the messages for 30 days after running the expunge command.

### Be cautious to save anything you don't want to lose. This is the disclaimer that *Mauler* is in the title of this project and it will do its best to reclaim your precious 5 GB quota, come hell or high water
