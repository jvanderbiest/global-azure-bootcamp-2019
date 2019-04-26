# Global Azure Bootcamp 2109 in Malta - talk about AI
This repository contains the examples and slides of the demo given at the bootcamp session on 27th of April 2019.

>The goal of the demo is to showcase how easy it is to create a bot from scratch with the [Microsoft BotFramework](https://dev.botframework.com/) SDK. We also do a basic integration with the [LUIS](https://www.luis.ai/) (Language Understanding) AI service and throw in some AI calls to [Microsoft Computer Vision](https://azure.microsoft.com/en-us/services/cognitive-services/computer-vision/).
Everything can be setup and run for free (based on the load, but for small projects you will be fine). If you don't have an Azure account, [get one for free](https://azure.microsoft.com/en-us/free/).

# Questions
If you have any questions, feel free to open a new issue.

## Setting up LUIS (NLP engine) 
Browse to the [LUIS portal](https://eu.luis.ai/) (is region specific, we use EU), login with your account and create a new application.

![Create LUIS app](/screens/1-create-app.png?raw=true "Create LUIS app")


Then navigate to intents under Build and create a new one named ´Registration´

![Create LUIS intent](/screens/2-create-intent.png?raw=true "Create LUIS intent")


Then click the intent and create some utterances. The utterance is what you would expect the user to send to the bot to end up with this intent. So the intent the user has with his input.

![Input LUIS utterances](/screens/3-registration-utterances.png?raw=true "Input LUIS utterances")


After that, you can create an entity by hovering your previously created utterance. We create a new simple entity named `eventType`. The entity extracts data from the utterance that is predictable.

![Create simple LUIS entity](/screens/4-create-simple-entity.png?raw=true "Create simple LUIS entity")

![Label utterance as entity](/screens/5-label-utterance-as-entity.png?raw=true "Label utterance as entity")


Let's train the app, so LUIS is up to date of all the changes we were making. You always need to train the app before you can test it.

![LUIS train application](/screens/6-train-app.png?raw=true "LUIS train application")


After training, let's test what we created so far. You should see that an utterance craeted before should match the `Registration` intent. It could also match the entity `eventType` if it contains an event type. Also try with utterances that are related but not a direct match in LUIS, it should give a lower score.

![LUIS test application](/screens/7-test-app.png?raw=true "LUIS test application")


When we want to use the LUIS functionality out of the portal, we should publish it to a slot. We can publish to Production and to Staging. If we publish to staging we need to add this parameter in the API request, so for this example we can just publish to Production.

![LUIS test application](/screens/8-publish-app.png?raw=true "LUIS test application")


## Connecting Azure to LUIS
If you want to use your cognitive resource from Azure and link it to your LUIS app so it can use that plan, you should first create a LUIS resource in Azure.

![Create LUIS resource in Azure](/screens/9-create-luis-resource-in-azure.png?raw=true "Create LUIS resource in Azure")

![Detailed LUIS resource in Azure](/screens/10-create-luis-resource-in-azure-2.png?raw=true "Create LUIS resource in Azure")


After that, you might want to relogin in LUIS and when you're using the same account as in the Azure Portal, you can link them.

![Link LUIS Azure resource in LUIS portal](/screens/11-assign-azure-resource-in-luis-button.png?raw=true "Link LUIS Azure resource in LUIS portal")

![Assign LUIS Azure resource in LUIS portal](/screens/12-assign-azure-resource-in-luis.png?raw=true "Assign LUIS Azure resource in LUIS portal")


# Start coding in Visual Studio 2019
To kickstart our project, we use the scaffolding templates provided by BotFramework. To use this, you can install the [VSIX](https://marketplace.visualstudio.com/items?itemName=BotBuilder.botbuilderv4) package.

![BotFramework Template](/screens/13-botframework-template.png?raw=true "BotFramework Template")


Create a new project using the `EmptyBot` option template

![BotFramework Template Project Template](/screens/14-botframework-project-type.png?raw=true "BotFramework Template Project Template")

![BotFramework Create Project](/screens/14-botframework-project-type-config.png?raw=true "BotFramework Create Project")


Run the bot by pressing F5 in Visual Studio, you should see something similar like this

![BotFramework Base Project](/screens/17-running-the-hello-world-bot.png?raw=true "BotFramework Base Project")


Use the snippets in the snippets folder in this repository.
We are editing the Startup class, your bot class and the appsettings file.


# Test the bot in the emulator
Download and install the latest [BotFramework emulator](https://github.com/Microsoft/BotFramework-Emulator/releases). Open the .bot file in the emulator that got created in the project.

![Emulator open bot](/screens/16-open-bot.png?raw=true "Emulator open bot")

![Running the bot](/screens/17-running-the-hello-world-bot.png?raw=true "Running the bot")


# Setup Azure Computer Vision
Create a new Computer Vision resource in Azure.

![Azure Computer Vision Creation](/screens/18-azure-computer-vision.png?raw=true "Azure Computer Vision Creation")

![Azure Computer Vision Configuration](/screens/19-azure-computer-vision-configuration.png?raw=true "Azure Computer Vision Configuration")


# Setup Azure Bing Spellcheck
Create a new Bing Spellcheck v7 resource in Azure.

![Azure Bing Spellcheck](/screens/20-azure-bing-spellchec.pngk?raw=true "Azure Bing Spellcheck")

![Azure Bing Spellcheck Configuration](/screens/21-azure-bing-spellcheck-configuration.png?raw=true "Azure Bing Spellcheck Configuration")


Enable Bing Spellcheck in LUIS. This is just a switch, you will use the spellcheck authorization key in the API call.

![Enable Bing Spellcheck in LUIS](/screens/21-luis-bing-spellcheck-enable.png?raw=true "Enable Bing Spellcheck in LUIS")


# Register Azure bot channel
To use the bot in the cloud, you will have to provision a bot channels registrations in Azure. In the registration you can configure the channels (facebook, slack, web, ...) that the bot will be accessible from. Make sure you configure the endpoint in the settings page. The endpoint contains the address to the bot messages API that will be called when a user starts talking to the bot.

![Register Azure bot channel](/screens/22-bot-channels-registration.png?raw=true "Register Azure bot channel")


The easiest is to directly create a new Microsoft App, it will handle the security of your bot.

![Microsoft App](/screens/23-microsoft-app-azure.png?raw=true "Microsoft App")

![Microsoft App Generate](/screens/24-generate-new-microsoft-app-password.png?raw=true "Microsoft App Generate")

![Microsoft App Generate](/screens/25-bot-channel-registration.png?raw=true "Microsoft App Generate")


# DirectLine Jabber
While preparing this demo, I've create an automated testing tool that was already some time in the back of my head. [You can read more about it](https://github.com/jvanderbiest/directline-jabber)