<h2>Welcome to AsyncSections</h2>

<h3>What problem do AsyncSections solve?</h3>
<p>Some serverside functions take a while to complete, this can cause IIS thread starvation, which is addressed by MVC's AsyncController. Great! :-)</p>
<p>However, from a user's perspective nothing changes. They still have to wait x seconds for the page to respond and are are usually stuck on a white page that looks like the server isnt doing anything :-(</p>
<p>That's where AsyncSections come in! Imagine if you could seperate your view into sections which contain the results of your long running functions. Now you can! These are seperated from normal document rendering to allow the page to load first, then load in the results when they are ready.</p>

<p>Check out the <a href="http://asyncsectionsdemo.atlascode.com/Comparison" target="_blank">comparison demo</a> to see AsyncSections in action.<p>

<h3>When would I use an AsyncSection</h3>
<p>Whenever you are doing something on the server that takes a few seconds but you dont want to make the user wait before allowing them to use the site. For example...</p>
<ul>
	<li>Processing an imported spreadsheet</li>
	<li>Waiting for 3rd party APIs to respond</li>
	<li>Doing some processing on all records in the database</li>
	<li>Generating documents</li>
	<li>Bulk functions like emailing all users or creating notification records</li>
	<li>Updating the database schema of the site using an EF 4.3 data migration :-)</li>
	<li>Use your imagination</li>
</ul>
<p>In theory, any page that accesses the database could use an AsyncSection, especially if it runs multiple different queries that display their results in different areas of the page, like a KPI dashboard</p>

<h3>How do AsyncSections work?</h3>
<p>AsyncSections are standard MVC sections which are suffixed with "Async" but they dont get rendered to the page in the standard way.</p>

<p>There is a new AsyncSectionController which contains the new AsyncView() and AsyncSection() methods. These combined with the new RenderBodyAsync() helper will split the page into multiple render stages.</p>
<ol>
	<li>MasterPage is rendered up to the call to RenderBodyAsync()</li>
	<li>The body is rendered excluding anything in an AsyncSection</li>
	<li>A number of AsyncSections are rendered</li>
	<li>The last half of the MasterPage is rendered after the call to RenderBodyAsync()</li>
</ol>

![](https://raw.github.com/atlascode/asyncsections/master/AtlasCode.AsyncSections.Demos/Content/AsyncSectionsBasics.png?raw=true)

<p>In the example above, you will see that the element in the Task1Async section will be rendered after the example footer</p>

<h3>But wait... what if I want my section to load into a specific part of the page?</h3>
<p>Good point! AsyncSections will nativly only get rendered just after your RenderBodyAsync() call. In order to load content into specific parts of the page you will need to employ some kind of javascript based solution (or css if your design permits). For this reason I have also created a small helper library calles jsTransform which may get its own repo one day if it gets enough use. You can see this in action in the Multiple Tasks Demo</p>
<p>jsTransform checks the page for new elements every 100ms until the document is ready. If the element has one of the transform attributes (transform-append, transform-replace or transform-removeClass) then it will act on the element and remove the attributes.</p>

![](https://raw.github.com/atlascode/asyncsections/master/AtlasCode.AsyncSections.Demos/Content/ConsoleDemo.png?raw=true)

<p>In the example above you can see that there is an unordered list with an id of consoleList and the AsyncSection contains a list item that has a transform-append attribute with a value of consoleList. This will take the list item and append it to the unordered list as soon as the list item arrives on the page.</p>
<p>Because AsyncSections arrive after the page has first rendrered, they will appear to blink at the bottom of the page until jsTransform moves them to their desired position in the DOM. To get arround this, I have added a hidden class to the list item that gets removed once the item element is transformed. There are many different ways to solve this problem, but this is a quick way to get you started.</p>
<p>The jsTransform library is currently quite small and only contains 3 attributes, but it will grow over time as people need more from it. Or alternatively, you can use any javascript library you like, its up to you!</p>

<h3>Summary</h3>
<p>AsyncSections is a research project that contains a lot of reflection on the internals of MVC4 and should not really be used in a production environment. The idea was to show what was possible with a few small tweaks of the Rendering system and with a bit of luck, get something similar added to a roadmap for MVC5+</p>
<p>For this reason, this project will probably only ever have a <a href="https://nuget.org/packages/AtlasCode.AsyncSections">pre release NuGet package</a></p>
<p>Please show your support for this project by sharing it with others and letting the MVC team know you would like a feature like this in the future. Fingers crossed :-)</p>
<p>If you got this far, thank you for taking the time to read this document</p>
