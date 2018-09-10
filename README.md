# Azure Sky Media v3

Welcome! This repository contains the multi-tenant Azure media solution sample that is deployed at http://www.skymedia.tv

As a sample illustration, the screenshot below is an introductory <a href="http://azure.microsoft.com/services/media-services/" target="_blank">Azure Media Services</a> stream that is playing within an Azure App Service web app via <a href="http://azure.microsoft.com/services/media-services/media-player/" target="_blank">Azure Media Player</a> integration. On-demand and live video content that is stored and managed in Azure Media Services is adaptively streamed, scaled and globally consumable across a wide range of devices and platforms.

![](https://skymedia.azureedge.net/docs/01.2-ApplicationIntroduction.png)

Refer to http://github.com/RickShahid/SkyMedia/wiki for additional screenshots of key application functionality, including:

* Multi-tenant, self-service account registration and profile management, including user, media and storage accounts

* Secure content upload, storage and processing via encoding, indexing, dynamic encryption and dynamic packaging

* Discover and extract actionable insights from your media content via integrated video / audio intelligence services

The following web application architecture overview diagram depicts the sample deployment at http://www.skymedia.tv

![](https://skymedia.azureedge.net/docs/02.3-ApplicationArchitecture.png)

To deploy the parameterized solution sample within your Azure subscription, click the "Deploy to Azure" button below:

<table>
  <tr>
    <td>
      <b>Global Services Template</b>
    </td>
    <td>
      <a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FRickShahid%2FSkyMedia%2Fmaster%2FResourceManager%2FTemplate.Global.json" title="Deploy Global Services" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"></a>
    </td>
    <td>
      <a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2FRickShahid%2FSkyMedia%2Fmaster%2FResourceManager%2FTemplate.Global.json" title="Visualize Global Services" target="_blank"><img src="http://armviz.io/visualizebutton.png"></a>
    </td>
  </tr>
  <tr>
    <td>
      <b>Regional Services Template</b>
    </td>
    <td>
      <a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FRickShahid%2FSkyMedia%2Fmaster%2FResourceManager%2FTemplate.Regional.json" title="Deploy Regional Services" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"></a>
    </td>
    <td>
      <a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2FRickShahid%2FSkyMedia%2Fmaster%2FResourceManager%2FTemplate.Regional.json" title="Visualize Regional Services" target="_blank"><img src="http://armviz.io/visualizebutton.png"></a>
    </td>
  </tr>
</table>

The following Azure platform services were leveraged to create this sample media application:

* **Active Directory (B2C)** - http://azure.microsoft.com/services/active-directory-b2c/

* **Storage** - http://azure.microsoft.com/services/storage/

* **Cosmos DB** - http://azure.microsoft.com/services/cosmos-db/

* **Event Grid** - http://azure.microsoft.com/services/event-grid/

* **Functions** - http://azure.microsoft.com/services/functions/

* **Media Services** - http://azure.microsoft.com/services/media-services/

  * **Encoding** - http://azure.microsoft.com/services/media-services/encoding/

  * **Streaming** - http://azure.microsoft.com/services/media-services/live-on-demand/

  * **Protection** - http://azure.microsoft.com/services/media-services/content-protection/
  
  * **Player** - http://azure.microsoft.com/services/media-services/media-player/

* **Content Delivery Network** - http://azure.microsoft.com/services/cdn/

* **Video Indexer** - http://azure.microsoft.com/services/cognitive-services/video-indexer/

  * **Cognitive Services** - http://azure.microsoft.com/services/cognitive-services/

  * **Search** - http://azure.microsoft.com/services/search/

* **Cognitive Search** - http://docs.microsoft.com/azure/search/cognitive-search-concept-intro

* **App Service** - http://azure.microsoft.com/services/app-service/

* **App Insights** - http://azure.microsoft.com/services/application-insights/

* **Traffic Manager** - http://azure.microsoft.com/services/traffic-manager/

* **Team Services** - http://azure.microsoft.com/services/visual-studio-team-services/

If you have any issues or suggestions, please let me know.

Thanks.

Rick Shahid

Azure M&E Solutions

Architect & Developer

rick.shahid@microsoft.com
