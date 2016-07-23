﻿using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.CodeGenerators;
using RazorLight.Host;

namespace RazorLight
{
	public class RazorLightCodeGenerator
    {
		private readonly RazorTemplateEngine _templateEngine;
		private readonly ConfigurationOptions _config;

		public RazorLightCodeGenerator(ConfigurationOptions options)
		{
			if(options == null)
			{
				throw new ArgumentNullException(nameof(options));
			}

			_config = options;
			_templateEngine = new RazorTemplateEngine(new LightRazorHost());
		}

		public string GenerateCode<T>(TextReader input, T model)
		{
			GeneratorResults generatorResults = null;
			try
			{
				generatorResults = _templateEngine.GenerateCode(input);

				if (!generatorResults.Success)
				{
					var builder = new StringBuilder();
					builder.AppendLine("Failed to parse an input:");

					foreach (RazorError error in generatorResults.ParserErrors)
					{
						builder.AppendLine($"{error.Message} (line {error.Location.LineIndex})");
					}

					throw new RazorLightException(builder.ToString());
				}
			}
			catch (Exception ex) when (!(ex is RazorLightException))
			{
				throw new RazorLightException("Failed to generate a language code. See inner exception", ex);
			}

			return generatorResults.GeneratedCode;
		}
	}
}