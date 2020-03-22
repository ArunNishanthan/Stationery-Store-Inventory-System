package com.arun.api.AsyncTask.Get;

import android.os.AsyncTask;

import com.arun.api.Model.RequisitionByDepartmentItem;
import com.google.gson.Gson;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;

public class RequisationAsyncTask extends AsyncTask<String, Void, JSONObject> {
    public IReqCallback icallback;

    public RequisationAsyncTask(IReqCallback icallback) {
        this.icallback = icallback;
    }

    @Override
    protected JSONObject doInBackground(String... Strings) {
        JSONObject jsonObj = null;
        StringBuilder response = new StringBuilder();
        try {
            URL url = new URL("http://10.0.2.2:1256/api/Item/RequisitionList");
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
    protected void onPostExecute(JSONObject jsonObj) {
        if (jsonObj == null)
            icallback.getRequisitionList(null);
        try {
            String context = (String) jsonObj.get("context");
            if (context.compareTo("set") == 0) {
                JSONArray jArray = (JSONArray)jsonObj.get("RequisitionLists");
                ArrayList<RequisitionByDepartmentItem> requisitionByDepartmentItems = new  ArrayList<>();
                if (jArray != null) {
                    for (int i=0;i<jArray.length();i++){
                        Gson gson = new Gson();
                        RequisitionByDepartmentItem requisitionByDepartmentItem = gson.fromJson(jArray.getJSONObject(i).toString(), RequisitionByDepartmentItem.class);
                        requisitionByDepartmentItems.add(requisitionByDepartmentItem);
                    }
                }
                icallback.getRequisitionList(requisitionByDepartmentItems);
            }
        } catch (Exception e) {
        }

    }

    public interface IReqCallback {
        void getRequisitionList(ArrayList<RequisitionByDepartmentItem> requisitionByDepartmentItems);
    }
}