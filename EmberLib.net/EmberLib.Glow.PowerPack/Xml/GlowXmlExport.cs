﻿/*
   EmberLib.net -- .NET implementation of the Ember+ Protocol

   Copyright (C) 2012-2019 Lawo GmbH (http://www.lawo.com).
   Distributed under the Boost Software License, Version 1.0.
   (See accompanying file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace EmberLib.Glow.PowerPack.Xml
{
   /// <summary>
   /// Provides XML export functionalities for Glow trees.
   /// </summary>
   public sealed class GlowXmlExport : IGlowVisitor<XmlWriter, object>
   {
      /// <summary>
      /// Converts the glow tree with the root <paramref name="glow"/> to XML,
      /// writing the XML to <paramref name="writer"/>.
      /// </summary>
      /// <param name="glow">The root of the Glow tree to convert.</param>
      /// <param name="writer">The XmlWriter instance to write XML to.</param>
      public static void Export(GlowContainer glow, XmlWriter writer)
      {
         var instance = new GlowXmlExport();

         glow.Accept(instance, writer);

         writer.WriteWhitespace(Environment.NewLine);
      }

      #region Implementation
      private GlowXmlExport()
      {
      }

      void ConvertNode(GlowNodeBase glow, XmlWriter writer)
      {
         if(glow.HasContents)
         {
            writer.WriteStartElement("contents");
            glow.Identifier.Do(value => writer.WriteElementString("identifier", value));
            glow.Description.Do(value => writer.WriteElementString("description", value));
            glow.IsRoot.Do(value => writer.WriteElementString("isRoot", XmlConvert.ToString(value)));
            glow.IsOnline.Do(value => writer.WriteElementString("isOnline", XmlConvert.ToString(value)));
            writer.WriteEndElement();
         }

         var children = glow.Children;
         if(children != null)
         {
            writer.WriteStartElement("children");
            children.Accept(this, writer);
            writer.WriteEndElement();
         }
      }

      void ConvertValue(string elementName, GlowValue value, XmlWriter writer)
      {
         var typeStr = ConvertParameterType(value.Type);

         if(typeStr != null)
         {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", typeStr);
            writer.WriteString(value.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();
         }
      }

      string ConvertParameterType(int parameterType)
      {
         switch(parameterType)
         {
            case GlowParameterType.Boolean: return "BOOLEAN";
            case GlowParameterType.Integer: return "INTEGER";
            case GlowParameterType.Octets:  return "OCTET STRING";
            case GlowParameterType.Real:    return "REAL";
            case GlowParameterType.String:  return "UTF8";
         }

         return null;
      }

      void ConvertMinMax(string elementName, GlowMinMax value, XmlWriter writer)
      {
         var typeStr = null as string;

         switch(value.Type)
         {
            case GlowParameterType.Integer:
               typeStr = "INTEGER";
               break;

            case GlowParameterType.Real:
               typeStr = "REAL";
               break;
         }

         if(typeStr != null)
         {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", typeStr);
            writer.WriteString(value.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();
         }
      }

      void ConvertParameter(GlowParameterBase glow, XmlWriter writer)
      {
         if(glow.HasContents)
         {
            writer.WriteStartElement("contents");
            glow.Identifier.Do(value => writer.WriteElementString("identifier", value));
            glow.Description.Do(value => writer.WriteElementString("description", value));

            var glowValue = glow.Value;
            if(glowValue != null)
               ConvertValue("value", glowValue, writer);

            var glowMinMax = glow.Minimum;
            if(glowMinMax != null)
               ConvertMinMax("minimum", glowMinMax, writer);

            glowMinMax = glow.Maximum;
            if(glowMinMax != null)
               ConvertMinMax("maximum", glowMinMax, writer);

            glowValue = glow.Default;
            if(glowValue != null)
               ConvertValue("default", glowValue, writer);

            glow.Enumeration.Do(value => writer.WriteElementString("enumeration", value));
            glow.Factor.Do(value => writer.WriteElementString("factor", XmlConvert.ToString(value)));
            glow.Format.Do(value => writer.WriteElementString("format", value));
            glow.Step.Do(value => writer.WriteElementString("step", XmlConvert.ToString(value)));
            glow.Access.Do(value => writer.WriteElementString("access", XmlConvert.ToString(value)));
            glow.Type.Do(value => writer.WriteElementString("type", XmlConvert.ToString(value)));
            glow.IsOnline.Do(value => writer.WriteElementString("isOnlin", XmlConvert.ToString(value)));
            glow.StreamIdentifier.Do(value => writer.WriteElementString("streamIdentifier", XmlConvert.ToString(value)));

            var enumMap = glow.EnumMap;
            if(enumMap != null)
               ConvertStringIntegerCollection("enumMap", enumMap, writer);

            var streamDescriptor = glow.StreamDescriptor;
            if(streamDescriptor != null)
               ConvertStreamDescription("streamDescriptor", streamDescriptor, writer);

            writer.WriteEndElement();
         }

         var children = glow.Children;
         if(children != null)
         {
            writer.WriteStartElement("children");
            children.Accept(this, writer);
            writer.WriteEndElement();
         }
      }

      void ConvertStringIntegerCollection(string elementName, GlowStringIntegerCollection map, XmlWriter writer)
      {
         writer.WriteStartElement(elementName);
         foreach(var pair in map.EnumEntries)
         {
            writer.WriteStartElement("StringIntegerPair");
            writer.WriteAttributeString("entryString", pair.EntryName);
            writer.WriteAttributeString("entryInteger", XmlConvert.ToString(pair.EntryInteger));
            writer.WriteEndElement();
         }
         writer.WriteEndElement();
      }

      void ConvertStreamDescription(string elementName, GlowStreamDescription glow, XmlWriter writer)
      {
         writer.WriteStartElement(elementName);
         writer.WriteElementString("format", XmlConvert.ToString(glow.Format));
         writer.WriteElementString("offset", XmlConvert.ToString(glow.Offset));
         writer.WriteEndElement();
      }

      string ConvertPath(int[] path)
      {
         var buffer = new StringBuilder();

         for(int index = 0; index < path.Length; index++)
         {
            if(index >= 1)
               buffer.Append(".");

            buffer.Append(XmlConvert.ToString(path[index]));
         }

         return buffer.ToString();
      }

      void ConvertMatrix(GlowMatrixBase glow, XmlWriter writer)
      {
         if(glow.HasContents)
         {
            writer.WriteStartElement("contents");
            glow.Identifier.Do(value => writer.WriteElementString("identifier", value));
            glow.Description.Do(value => writer.WriteElementString("description", value));
            glow.MatrixType.Do(value => writer.WriteElementString("type", XmlConvert.ToString(value)));
            glow.AddressingMode.Do(value => writer.WriteElementString("addressingMode", XmlConvert.ToString(value)));
            glow.TargetCount.Do(value => writer.WriteElementString("targetCount", XmlConvert.ToString(value)));
            glow.SourceCount.Do(value => writer.WriteElementString("sourceCount", XmlConvert.ToString(value)));
            glow.MaximumTotalConnects.Do(value => writer.WriteElementString("maximumTotalConnects", XmlConvert.ToString(value)));
            glow.MaximumConnectsPerTarget.Do(value => writer.WriteElementString("maximumConnectsPerTarget", XmlConvert.ToString(value)));

            var parametersLocation = glow.ParametersLocation;
            if(parametersLocation != null)
            {
               writer.WriteStartElement("parametersLocation");
               if(parametersLocation.Kind == GlowParametersLocationKind.BasePath)
               {
                  writer.WriteAttributeString("type", "RELATIVE-OID");
                  writer.WriteString(ConvertPath(parametersLocation.BasePath));
               }
               else if(parametersLocation.Kind == GlowParametersLocationKind.Inline)
               {
                  writer.WriteAttributeString("type", "INTEGER");
                  writer.WriteString(XmlConvert.ToString(parametersLocation.Inline));
               }
               writer.WriteEndElement();
            }

            glow.GainParameterNumber.Do(value => writer.WriteElementString("gainParameterNumber", XmlConvert.ToString(value)));

            var labels = glow.TypedLabels;
            if(labels != null)
            {
               writer.WriteStartElement("labels");

               foreach(var label in labels)
               {
                  writer.WriteStartElement("Label");
                  writer.WriteAttributeString("basePath", ConvertPath(label.BasePath));
                  writer.WriteAttributeString("description", label.Description);
                  writer.WriteEndElement();
               }

               writer.WriteEndElement();
            }

            writer.WriteEndElement();
         }

         var children = glow.Children;
         if(children != null)
         {
            writer.WriteStartElement("children");
            children.Accept(this, writer);
            writer.WriteEndElement();
         }

         var targets = glow.TypedTargets;
         if(targets != null)
         {
            writer.WriteStartElement("targets");
            foreach(var target in targets)
            {
               writer.WriteStartElement("Target");
               writer.WriteAttributeString("number", XmlConvert.ToString(target.Number));
               writer.WriteEndElement();
            }
            writer.WriteEndElement();
         }

         var sources = glow.TypedSources;
         if(sources != null)
         {
            writer.WriteStartElement("sources");
            foreach(var source in sources)
            {
               writer.WriteStartElement("Source");
               writer.WriteAttributeString("number", XmlConvert.ToString(source.Number));
               writer.WriteEndElement();
            }
            writer.WriteEndElement();
         }

         var connections = glow.TypedConnections;
         if(connections != null)
         {
            writer.WriteStartElement("connections");
            foreach(var connection in connections)
            {
               writer.WriteStartElement("Connection");
               writer.WriteAttributeString("target", XmlConvert.ToString(connection.Target));

               var connectionSources = connection.Sources;
               if(connectionSources != null)
                  writer.WriteElementString("sources", ConvertPath(connectionSources));

               connection.Operation.Do(value => writer.WriteElementString("operation", XmlConvert.ToString(value)));
               connection.Disposition.Do(value => writer.WriteElementString("disposition", XmlConvert.ToString(value)));
               writer.WriteEndElement();
            }
            writer.WriteEndElement();
         }
      }

      void ConvertFunction(GlowFunctionBase glow, XmlWriter writer)
      {
         if(glow.HasContents)
         {
            writer.WriteStartElement("contents");
            glow.Identifier.Do(value => writer.WriteElementString("identifier", value));
            glow.Description.Do(value => writer.WriteElementString("description", value));

            var arguments = glow.TypedArguments;
            if(arguments != null)
            {
               writer.WriteStartElement("arguments");
               ConvertTupleDescription(arguments, writer);
               writer.WriteEndElement();
            }

            var result = glow.TypedResult;
            if(result != null)
            {
               writer.WriteStartElement("result");
               ConvertTupleDescription(result, writer);
               writer.WriteEndElement();
            }

            writer.WriteEndElement();
         }

         var children = glow.Children;
         if(children != null)
         {
            writer.WriteStartElement("children");
            children.Accept(this, writer);
            writer.WriteEndElement();
         }
      }

      void ConvertTupleDescription(IEnumerable<GlowTupleItemDescription> tupleDescription, XmlWriter writer)
      {
         foreach(var itemDescriptor in tupleDescription)
         {
            writer.WriteStartElement("TupleItemDescription");
            writer.WriteElementString("type", ConvertParameterType(itemDescriptor.Type));
            itemDescriptor.Name.Do(value => writer.WriteElementString("name", value));
            writer.WriteEndElement();
         }
      }
      #endregion

      #region IGlowVisitor<XmlWriter,object> Members
      object IGlowVisitor<XmlWriter, object>.Visit(GlowCommand glow, XmlWriter state)
      {
         state.WriteStartElement("Command");
         state.WriteAttributeString("number", XmlConvert.ToString(glow.Number));
         glow.DirFieldMask.Do(value => state.WriteElementString("dirFieldMask", XmlConvert.ToString(value)));

         var invocation = glow.Invocation;
         if(invocation != null)
         {
            state.WriteStartElement("invocation");
            invocation.InvocationId.Do(value => state.WriteElementString("invocationId", XmlConvert.ToString(value)));

            var arguments = invocation.ArgumentValues;
            if(arguments != null)
            {
               state.WriteStartElement("arguments");
               foreach(var value in arguments)
                  ConvertValue("Value", value, state);
               state.WriteEndElement();
            }

            state.WriteEndElement();
         }

         state.WriteEndElement();
         return null;
      }

      object IGlowVisitor<XmlWriter, object>.Visit(GlowElementCollection glow, XmlWriter state)
      {
         foreach(var element in glow.Elements)
            element.Accept(this, state);

         return null;
      }

      object IGlowVisitor<XmlWriter, object>.Visit(GlowNode glow, XmlWriter state)
      {
         state.WriteStartElement("Node");
         state.WriteAttributeString("number", XmlConvert.ToString(glow.Number));

         ConvertNode(glow, state);

         state.WriteEndElement();
         return null;
      }

      object IGlowVisitor<XmlWriter, object>.Visit(GlowParameter glow, XmlWriter state)
      {
         state.WriteStartElement("Parameter");
         state.WriteAttributeString("number", XmlConvert.ToString(glow.Number));

         ConvertParameter(glow, state);

         state.WriteEndElement();
         return null;
      }

      object IGlowVisitor<XmlWriter, object>.Visit(GlowStreamCollection glow, XmlWriter state)
      {
         state.WriteStartElement("Root");
         state.WriteAttributeString("type", "StreamCollection");

         foreach(var entry in glow.StreamEntries)
         {
            state.WriteStartElement("StreamEntry");
            state.WriteAttributeString("streamIdentifier", XmlConvert.ToString(entry.StreamIdentifier));
            ConvertValue("streamValue", entry.StreamValue, state);
            state.WriteEndElement();
         }

         state.WriteEndElement();
         return null;
      }

      object IGlowVisitor<XmlWriter, object>.Visit(GlowQualifiedParameter glow, XmlWriter state)
      {
         state.WriteStartElement("QualifiedParameter");
         state.WriteAttributeString("path", ConvertPath(glow.Path));

         ConvertParameter(glow, state);

         state.WriteEndElement();
         return null;
      }

      object IGlowVisitor<XmlWriter, object>.Visit(GlowQualifiedNode glow, XmlWriter state)
      {
         state.WriteStartElement("QualifiedNode");
         state.WriteAttributeString("path", ConvertPath(glow.Path));

         ConvertNode(glow, state);

         state.WriteEndElement();
         return null;
      }

      object IGlowVisitor<XmlWriter, object>.Visit(GlowRootElementCollection glow, XmlWriter state)
      {
         state.WriteStartElement("Root");
         state.WriteAttributeString("type", "ElementCollection");

         foreach(var element in glow.Elements)
            element.Accept(this, state);

         state.WriteEndElement();
         return null;
      }

      object IGlowVisitor<XmlWriter, object>.Visit(GlowMatrix glow, XmlWriter state)
      {
         state.WriteStartElement("Matrix");
         state.WriteAttributeString("number", XmlConvert.ToString(glow.Number));

         ConvertMatrix(glow, state);

         state.WriteEndElement();
         return null;
      }

      object IGlowVisitor<XmlWriter, object>.Visit(GlowQualifiedMatrix glow, XmlWriter state)
      {
         state.WriteStartElement("QualifiedMatrix");
         state.WriteAttributeString("path", ConvertPath(glow.Path));

         ConvertMatrix(glow, state);

         state.WriteEndElement();
         return null;
      }

      object IGlowVisitor<XmlWriter, object>.Visit(GlowFunction glow, XmlWriter state)
      {
         state.WriteStartElement("Function");
         state.WriteAttributeString("number", XmlConvert.ToString(glow.Number));

         ConvertFunction(glow, state);

         state.WriteEndElement();
         return null;
      }

      object IGlowVisitor<XmlWriter, object>.Visit(GlowQualifiedFunction glow, XmlWriter state)
      {
         state.WriteStartElement("QualifiedFunction");
         state.WriteAttributeString("path", ConvertPath(glow.Path));

         ConvertFunction(glow, state);

         state.WriteEndElement();
         return null;
      }

      object IGlowVisitor<XmlWriter, object>.Visit(GlowInvocationResult glow, XmlWriter state)
      {
         state.WriteStartElement("Root");
         state.WriteAttributeString("type", "InvocationResult");
         state.WriteAttributeString("invocationId", XmlConvert.ToString(glow.InvocationId));

         glow.Success.Do(value => state.WriteElementString("success", XmlConvert.ToString(value)));

         var result = glow.ResultValues;
         if(result != null)
         {
            state.WriteStartElement("result");
            foreach(var value in result)
               ConvertValue("Value", value, state);
            state.WriteEndElement();
         }

         state.WriteEndElement();
         return null;
      }

      object IGlowVisitor<XmlWriter, object>.Visit(GlowSubContainer glow, XmlWriter state)
      {
         return null;
      }

      object IGlowVisitor<XmlWriter, object>.Visit(GlowTemplate glow, XmlWriter state)
      {
          state.WriteStartElement("Template");
          {
              state.WriteAttributeString("number", glow.Number.ToString());

              if (glow.Description != null)
              {
                  state.WriteAttributeString("description", glow.Description);
              }

              if (glow.Element != null)
              {
                  state.WriteStartElement("element");
                  glow.Element.Accept(this, state);
                  state.WriteEndElement();
              }
          }
          state.WriteEndElement();
          return null;
      }

      object IGlowVisitor<XmlWriter, object>.Visit(GlowQualifiedTemplate glow, XmlWriter state)
      {
          state.WriteStartElement("QualifiedTemplate");
          {
              state.WriteAttributeString("path", ConvertPath(glow.Path));

              if (glow.Description != null)
              {
                  state.WriteAttributeString("description", glow.Description);
              }

              if (glow.Element != null)
              {
                  state.WriteStartElement("element");
                  glow.Element.Accept(this, state);
                  state.WriteEndElement();
              }
          }
          state.WriteEndElement();
          return null;
      }
      #endregion
   }
}
