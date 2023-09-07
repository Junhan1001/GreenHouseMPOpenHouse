/*
 * Include the lodash module for convenient functions such as "random"
 *  Check https://docs.unity.com/cloud-code/manual/scripts/reference/available-libraries
 *  to get a full list of libraries you can import in Cloud Code
 */

 // This code needs to be put into Unity Dashboard / Cloud Code / Scripts / Script Code. The script name will be "Restriction".
 // With a parameter named "Date_To_Unlock" and type "String".
const _ = require('lodash-4.17')
const { DataApi } = require("@unity-services/cloud-save-1.3")
const adminId = 'UifEvKaeH4ndL0QsZqLLViR2cNl6'

module.exports = async ({ params, context, logger }) => {
  const { projectId, playerId } = context;
  const api = new DataApi(context);
  const adminHadSetData = await api.getItems(projectId, adminId, "DateUnlock");

  if (params['Date_To_Unlock'] == undefined){
    const adminSetDateData = await api.getItems(projectId, adminId, "DateUnlock");
    const adminSetDate = adminSetDateData.data.results[0]?.value || 0;
    
    if (adminSetDate == 0){
      // No date found
      return{
        'Date unlock': "null"
      }
    }
    else{
      return {
        'Date unlock': adminSetDate
      }
    }
  }
  else{
    // Had not set date before
    const setDate = {key: "DateUnlock", value: (params['Date_To_Unlock'])};
    const setResult = await api.setItem(projectId, adminId, setDate);
    return{
      'Set Date': setDate.value
    }
  }
}