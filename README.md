# VeraCustomTriage

Do you want to:
- Add your own guidance to Veracode flaws?
- Add internal workflow information, wikis, TSRV templates?
- Do you want to add contact information for a 2nd party module that is littered across your estate?
- Or how about a different language against flaws?

There are a ton of reasons to want to add bespoke guidance to flaws, that is what this tool is for.

VeraCustomTriage reads in JSON configuration files which are then used to augment the output of a Veracode scan and put the results into a spreadsheet, within an encrypted zip file.

## Suggested usage
- Security team to create a repository that contains the global configuration JSON file.
- Security team and development teams make pull requests to the global configuration, which can be reviewed and approved by the security team.
- These global comments can then be added to all augmented flaw reports.

Provide bespoke relevant advice, an "open" and change controlled remediation repository, and promotes collaboration through code.

## JSON Configuration

Please see this example of a JSON configuration file

https://raw.githubusercontent.com/sebcoles/VeraCustomTriage/master/VeraCustomTriage/customtriage.global.json

The JSON configuration is setup with Property Conditions, this is a key value pair based on a property of the XML triage flaws output and what to watch for in the value. You can setup multiple property conditions per custom response, allowing specific flaws, in a specific class/file and module to generate a custom response.

For example, the below configuration would add a comment into the spreadsheet for ever CWE 73 flaws found in a scan.

```json
 "AutoResponse": [
    {
      "Title": "CWE 73 Wiki",
      "Response": "Please read this https://community.veracode.com/s/article/how-do-i-fix-cwe-73-external-control-of-file-name-or-path-in-java.",
      "PropertyConditions": [
        {
          "Property": "cweid",
          "Condition": "73",
          "ActionToTake": "Fix"
        }
      ]
    }
  ]
```

Or perhaps you are watching out for a 2nd party module?

```
 "AutoResponse": [
    {
      "Title": "Out of date 2nd party module",
      "Response": "This flaw has been fixed in 1.2, you need to upgrade your jam module, read the docs http://some.internal.wiki",
      "PropertyConditions": [
        {
          "Property": "line",
          "Condition": "12",
          "ActionToTake": "Upgrade"
        },
        {
          "Property": "class",
          "Condition": "jam.class",
          "ActionToTake": "Upgrade"
        },
        {
          "Property": "module",
          "Condition": "jam.jar",
          "ActionToTake": "Upgrade"
        }
      ]
    }
  ]
```

Up to 3 JSON configuration can be used.

## Appsettings.json
The appsettings within the solution can also provide additional configuration.

### How to use
Update the appsettings.json to include 3 JSON configuration files.

Update the appsettings.json with location of the Veracode credentials file.

You can then also filter out which flaws to include in the remediation spreadsheet that is generated by configuring a FlawFilter in the `appsettings.json`. For example the below configuration will only include flaws that are severity "High" or "Very High" and the remediation status is "Open" or "Reopened"

```
 "VeracodeFileLocation": "%userprofile%\\.veracode\\credentials",
  "Endpoint": {
    "Global": "https://raw.githubusercontent.com/sebcoles/VeraCustomTriage/master/VeraCustomTriage/customtriage.global.json",
    "Team": "https://raw.githubusercontent.com/sebcoles/VeraCustomTriage/master/VeraCustomTriage/customtriage.team.json",
    "Personal": "https://raw.githubusercontent.com/sebcoles/VeraCustomTriage/master/VeraCustomTriage/customtriage.personal.json"
  },
  "FlawFilterConfiguration": {
    "PropertyConditionCollections": [
      {
        "PropertyConditions": [
          {
            "Property": "severity",
            "Condition": "5"
          },
          {
            "Property": "severity",
            "Condition": "4"
          }
        ]
      },
      {
        "PropertyConditions": [
          {
            "Property": "remediation_status",
            "Condition": "Open"
          },
          {
            "Property": "remediation_status",
            "Condition": "Reopened"
          }
        ]
      }
    ]
  }
```

Once the above is configured you can then run the executable providing the `appid` `buildid` and a `password` for the zip file.

`VeraCustomTriage.Console.exe triage -a {YOUR_APP_ID} -s {YOUR_BUILD_ID} -p {A_PASS_FOR_ZIP_FILE}`

This will generate an encrypted zip file, within is a spreadsheet with the filtered flaws with a column for bespoke feedback read from the configuration file.

Forever in alpha, please feel free to fork or raise issues.

