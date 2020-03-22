package com.arun.api.AsyncTask.Get;

import android.os.AsyncTask;

import com.arun.api.Model.RetrievalList;

import com.google.gson.Gson;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;

public class RetrievalAsyncTask extends AsyncTask<String, Void,JSONObject>
{
    public ICallback callback;

    public RetrievalAsyncTask(ICallback callback)
    {
        this.callback= callback;
    }

    @Override
    protected JSONObject doInBackground(String... strings) {
        JSONObject jsonObj = null;
        StringBuilder response = new StringBuilder();
        try {
            URL url = new URL("http://10.0.2.2:1256/api/Item/RetrievalLists");
            HttpURLConnection conn = (HttpURLConnection) url.openConnection();
            conn.setRequestMethod("GET");
            // receive response
            try {
                InputStream inputStream = new BufferedInputStream(conn.getInputStream());
                BufferedReader r = new BufferedReader(new InputStreamReader(inputStream));
                for (String line; (line = r.readLine()) != null; ) {
                    response.append(line).append('\n');
                }
                jsonObj = new JSONObject(response.toString());
                jsonObj.put("context", "set");
            } catch (Exception e) {
                e.printStackTrace();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return jsonObj;
    }

    @Override
    protected void onPostExecute(JSONObject jsonObj)
    {
        String ReqFormID="";
        if (jsonObj!= null)
        {
                    JSONArray RetrievalJsonArray = null;
                    ArrayList<RetrievalList> retrievalLists= new ArrayList<>();
                    try
                    {
                        RetrievalJsonArray = jsonObj.getJSONArray("RetrivalLists");
                        ReqFormID =  jsonObj.getString("RequisationFormsID");
                    }
                    catch (JSONException e)
                    {
                        e.printStackTrace();
                    }

                    for(int i = 0; i < RetrievalJsonArray.length(); i++)
                    {
                        Gson gson = new Gson();
                        RetrievalList retrivalList= null;
                        try
                        {
                            retrivalList = gson.fromJson(RetrievalJsonArray.getJSONObject(i).toString(), RetrievalList.class);
                        }
                        catch (JSONException e)
                        {
                            e.printStackTrace();
                        }
                        retrievalLists.add(retrivalList);
                    }
                    callback.SentRetrievalList(retrievalLists,ReqFormID);
        }

    }

    public interface ICallback
    {
         void SentRetrievalList(ArrayList<RetrievalList> list, String formID);
    }
}
