const jsdom = require("jsdom");
var converter = require('./xml2json.js');
const fs = require('fs');


const { JSDOM } = jsdom;
const { window } = new JSDOM(`...`);

function parseXml(xml) {
   var dom = null;
   if (window.DOMParser) {
      try {
         dom = (new window.DOMParser()).parseFromString(xml, "text/xml");
      }
      catch (e) { dom = null; }
   }
   else if (window.ActiveXObject) {
      try {
         dom = new ActiveXObject('Microsoft.XMLDOM');
         dom.async = false;
         if (!dom.loadXML(xml)) // parse error ..

            window.alert(dom.parseError.reason + dom.parseError.srcText);
      }
      catch (e) { dom = null; }
   }
   else
      alert("cannot parse xml string!");
   return dom;
}

function convertJsonToDefinitions(json) {
   var definitions = [];

   function createInterface(json, interfaceName) {
      function capitalize(str) {
         return str.replace(/\b\w/g, l => l.toUpperCase());
      }

      var definition = 'interface ' + capitalize(interfaceName) + ' {\n';
      Object.keys(json).forEach((key) => {
         if (key.startsWith("@")) {
            return;
         }
         if (typeof json[key] === "object") {
            var newInterface = capitalize(interfaceName) + "_" + capitalize(key);
            definition += '    ' + key + ': ' + newInterface + ';\n';
            createInterface(json[key], newInterface);
         } else {
            definition += '    /** ' + json[key] + ' */\n';
            definition += '    ' + key + ': string;\n';
         }
      });

      definition += '}\n\n';
      definitions.push(definition);
   }
   createInterface(json, 'ReviewResorces');
   return definitions;
}

console.log(
`---------------------------------------------------------------------------------------
The script generates:
* fake JSON file used to initialize resources store in storybook
* TypeScript definitions that allows to have strongly typed access to resources.

It's reading the embeded english version XML file from EPiserver project.
---------------------------------------------------------------------------------------
`); 


console.log('Reading Episerver XML file');
fs.readFile('./../AdvancedApprovalReviews/EmbededLanguages/advancedapprovalreviews_EN.xml', 'utf-8', (err, data) => {
   if (err) { throw err; }

   console.log('Parsing string to XML');
   var xml = parseXml(data);

   console.log('Converting XML to JSON string');
   var jsonStr = converter.xmlConverter(xml, ' ');

   console.log('Parsing JSON string');
   var json = JSON.parse(jsonStr);

   const jsonRoot = json.languages.language.reviewcomponent

   console.log('Saving JSON file');
   fs.writeFile('./stories/resources.json', JSON.stringify(jsonRoot, null, 2), (err) => {
      if (err) throw err;
      console.log('JSON resources generated');
   });

   console.log('Converting JSON to typescript definitions');
   var definitions = convertJsonToDefinitions(jsonRoot);

   console.log('Saving file');
   fs.writeFile('./resources.d.ts', definitions.join(''), (err) => {
      if (err) throw err;
      console.log('Resources definition file generated');
   });
});

