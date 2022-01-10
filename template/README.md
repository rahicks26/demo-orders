# Initial Solution

We will start our talk here. You will notice that we have defined a few types and got a pretty simple but fully functional API up. We also have some tests to talk about and a few we will write.

Now let talk about what we want to focus on today. Today we will cover testing APIs with Expecto. Expecto is generally used for writing unit tests, but as you will soon see it is capable of doing so much more.

## Why test your API

 Have you been asked why test an API? It can be a difficult question to answer honestly. After all, every test we write only gives us more code that we have to maintain, and not sure your test code will solve business problems. Or does it?

- Would it be nice if we knew our change would break a client application? 
- Would you like to know if you used your web framework correctly? 
- Would you like to have better documentation around your API? 
- Would you like to know if your code solves the right business problem?

Each of the questions above **can** be answered to some extent with aid of tests introduced at the API level. Don't be misled though testing at this level can also be an absolute waste of time in some cases, so take everything we talk about here with caution. 

## Ways to test our API

There are a few ways we want to test our API. 

- Test the public contract it creates
    - Checks resources names
    - Checks HTTP verbs
    - Checks Request and Response shapes
- Test business flows without requiring persistence
- Test units of our API
    - HTTP handlers
    - Request and response mappings
- Test our API end to end with persistence

We will use Expecto for each of these, but they each have pros and cons.
