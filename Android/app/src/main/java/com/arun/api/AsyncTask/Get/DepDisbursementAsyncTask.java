package com.arun.api.AsyncTask.Get;

import android.os.AsyncTask;

import com.arun.api.Model.DepDisbursementList;
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

public class DepDisbursementAsyncTask extends AsyncTask<String, Void,JSONObject> {
    public ICallback callback;

    public DepDisbursementAsyncTask(ICallback callback)
    {
        this.callback= callback;
    }

    public DepDisbursementAsyncTask() {

    }

    @Override
    protected JSONObject doInBackground(String... strings) {
        JSONObject jsonObj = null;
        StringBuilder response = new StringBuilder();
        try {
            URL url = new URL("http://10.0.2.2:1256/api/Item/DeptDisburmentLists");

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
                //e.printStackTrace();
                e.getMessage();
            }
        } catch (Exception e) {
            e.printStackTrace();
           // e.getMessage();
        }
        return jsonObj;
    }

    @Override
    protected void onPostExecute(JSONObject jsonObj)
    {
        if (jsonObj!= null)
        {
            JSONArray DepDisbursementListJsonArray = null;
            ArrayList<DepDisbursementList> depDisbursementLists= new ArrayList<>();
            try
            {
                DepDisbursementListJsonArray = jsonObj.getJSONArray("DepDisbursementLists");

            }
            catch (JSONException e)
            {
                e.printStackTrace();
            }

            for(int i = 0; i < DepDisbursementListJsonArray.length(); i++)
            {
                Gson gson = new Gson();
                DepDisbursementList depDisbursementList= null;
                try
                {
                    depDisbursementList = gson.fromJson(DepDisbursementListJsonArray.getJSONObject(i).toString(), DepDisbursementList.class);
                }
                catch (JSONException e)
                {
                    e.printStackTrace();
                }

                depDisbursementLists.add(depDisbursementList);
            }
            callback.sendDepDisbursementList(depDisbursementLists);
        }

    }

    public interface ICallback
    {
        void sendDepDisbursementList(ArrayList<DepDisbursementList> list);
    }
}
