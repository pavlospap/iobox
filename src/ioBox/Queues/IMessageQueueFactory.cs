namespace IOBox.Queues;

internal interface IMessageQueueFactory
{
    IMessageQueue GetOrCreate(string key);
}
