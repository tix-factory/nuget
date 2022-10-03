# TixFactory.Firebase

## Code Samples

Initialize classes

```csharp
var firebaseServerKey = "your firebase server key, however you get this";

var httpClient = new HttpClient();
httpClient.Handlers.Insert(0, new FirebaseAuthorizationHandler(firebaseServerKey)); // Add FirebaseAuthorizationHandler to add authorizations to all outbound Firebase requests

var firebaseMessageSender = new FirebaseMessageSender(httpClient);
var firebaseTopicSubscriptionManager = new FirebaseTopicSubscriptionManager(httpClient);
```

Send message

```csharp
var topic = "example-topic";
object exampleMessageData = new
{
	Hello = "world"
};

firebaseMessageSender.Send(topic, exampleMessageData);
```

Subscribe to topics

```csharp
var topic = "example-topic";
var token = "the token to subscribe to the topic";

firebaseTopicSubscriptionManager.Subscribe(token, topic); // Subscribe to topic

ICollection<string> topicsSubscribedTo = firebaseTopicSubscriptionManager.GetSubscribedTopics(token); // Get list of topics token is subscribed to

firebaseTopicSubscriptionManager.Unsubscribe(token, topic); // Unsubscribe from topic
```
