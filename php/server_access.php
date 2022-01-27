<?php

//クライアントからのリクエストを処理
process_req($_POST["req"], $_POST["data"]);

//リクエストの処理&応答
function process_req(string $req, string $data)
{
    if($req == "get_device") { echo get_device(); }
    else if($req == "get_extra") { echo get_extra(); }
    else if($req == "update") { echo update($data); }
    else if($req == "insert") { echo insert($data); }
    else if($req == "remove") { echo remove($data); }
    else if($req == "login") { echo login_user($data); }
    else if($req == "regist") { echo regist_user($data); }
    else if($req == "arrow_register") { echo arrow_register($data); }
    else { echo "unable to process request."; }
}

//PDO(MySQL接続)
function connect_db()
{
    //ユーザ名、パスワード、DBアドレス
    $dsn = 'mysql:dbname=***;host=***;charset=utf8';
    $user = '***';
    $pass = '***';
    //pdo接続
    try { $pdo = new PDO($dsn, $user, $pass); }
    catch (PDOException $e) { exit('' . $e->getMessage()); }
    //実行結果を返す
    return $pdo;
}

function login_user(string $data)
{
    $pdo = connect_db();
    //データ型は"device_id"
    try { $users = $pdo->query("SELECT * FROM user WHERE device_id = '$data' AND block = 0"); }
    catch(PDOException $e) { var_dump($e->getMessage()); }
    if($users->rowCount() > 0) return "can use.";
    else return "cannot use.";
}

function regist_user(string $data)
{
    $pdo = connect_db();
    //データ型は"device_id|student_id"
    $d1 = explode("|", $data);
    $did = $d1[0];
    $sid = $d1[1];
    try { $pdo->query("INSERT INTO user (device_id, student_id) VALUES ('$did', '$sid')"); }
    catch(PDOException $e) { var_dump($e->getMessage()); }
    return "regist complete.";
}

//パスワード照会（初回のみ）
function arrow_register(string $data)
{
    $pdo = connect_db();
    $ky = '***'; // 16 * 8 = 128 bit key
	$iv = '***'; // 16 * 8 = 128 bit iv
    $pw = openssl_decrypt($data, 'AES-128-CBC', $ky, 0, $iv);
    try { $stmt = $pdo->query("SELECT * FROM regist"); }
    catch(PDOException $e) { var_dump($e->getMessage()); }
    foreach($stmt as $row)
    {
        $password = $row["password"];
        if($password == $pw) return "register is arrowed.";
        else break;
    }
    return "register is not arrowed.";
}

//device取得
function get_device()
{
    $ret = "";
    //データ取得
    try { $stmt = connect_db()->query("SELECT * FROM device"); }
    catch (PDOException $e) { var_dump($e->getMessage()); }
    //変換
    foreach($stmt as $row)
    {
        $id = $row["id"];
        $aut = $row["author"];
        $cla = $row["class"];
        $clde = $row["class_detail"];
        $ald = $row["ald"];
        $sul = $row["sulfurization"];
        $eff = $row["efficiency"];
        $ff = $row["ff"];
        $voc = $row["voc"];
        $jsc = $row["jsc"];
        $lab = $row["lab"];
        $url = $row["url"];
        $param = "$id=$aut=$cla=$clde=$ald=$sul=$eff=$ff=$voc=$jsc=$lab^$url";
        $ret = $ret == "" ? "$param" : "$ret|$param";
    }
    if($ret == "") var_dump("unable to get device data.");
    return $ret;
}

//extra取得
function get_extra()
{
    $ret = "";
    //データ取得
    try { $stmt = connect_db()->query("SELECT * FROM extra"); }
    catch (PDOException $e) { var_dump($e->getMessage()); }
    //変換
    foreach($stmt as $row)
    {
        $did = $row["device_id"];
        $tag = $row["tag"];
        $val = $row["value"];
        $param = "$did=$tag=$val";
        $ret = $ret == "" ? "$param" : "$ret|$param";
    }
    if($ret == "") var_dump("unable to get extra data.");
    return $ret;
}

//更新
function update(string $data)
{
    $pdo = connect_db();
    //データ型は"id|aut=...=lab|url|tag=val|tag=val..."
    $d1 = explode("|", $data);
    $id = $d1[0];
    try { $extras = $pdo->query("SELECT * FROM extra WHERE device_id = $id"); }
    catch(PDOException $e) { var_dump($e->getMessage()); }
    $exs = $extras->fetchAll();
    $processed_tag = array();
    for($i = 1; $i < count($d1); $i++)
    {
        $ed = explode("=", $d1[$i]);
        if($i == 1)
        {
            $aut = $ed[0];
            $cla = $ed[1];
            $clde = $ed[2];
            $ald = $ed[3];
            $sul = $ed[4];
            $eff = $ed[5];
            $ff = $ed[6];
            $voc = $ed[7];
            $jsc = $ed[8];
            $lab = $ed[9];
            //更新
            try
            {
                $pdo->query("UPDATE device
                            SET author = '$aut', class = '$cla', class_detail = '$clde',
                            ald = '$ald', sulfurization = '$sul', efficiency = $eff, ff = $ff,
                            voc = $voc, jsc = $jsc, lab = $lab
                            WHERE id = $id");
            }
            catch(PDOException $e) { var_dump($e->getMessage()); }
        }
        else if($i == 2)
        {
            $url = $ed[0];
            try { $pdo->query("UPDATE device SET url = '$url' WHERE id = $id"); }
            catch(PDOException $e) { var_dump($e->getMessage()); }
        }
        else
        {
            $tag = $ed[0];
            $val = $ed[1];
            //更新か追加か確認
            $upd = false;
            foreach($exs as $row)
            {
                if($row["tag"] == $tag)
                {
                    $upd = true;
                    $processed_tag[] = $tag;
                    break;
                }
            }
            if($upd)
            {
                //更新
                try { $pdo->query("UPDATE extra SET value = $val WHERE device_id = $id AND tag = '$tag'"); }
                catch(PDOException $e) { var_dump($e->getMessage()); }
                
            }
            else
            {
                //追加
                try { $pdo->query("INSERT INTO extra (device_id, tag, value) VALUES ($id, '$tag', $val)"); }
                catch(PDOException $e) { var_dump($e->getMessage()); }
            }
        }
    }
    //更新、追加されていないタグを削除
    foreach($exs as $row2)
    {
        $tag = $row2["tag"];$del = true;
        foreach($processed_tag as $ptag)
        {
            if($tag == $ptag)
            {
                $del = false;
                break;
            }
        }
        if($del)
        {
            try { $pdo->query("DELETE FROM extra WHERE device_id = $id AND tag = '$tag'"); }
            catch(PDOException $e) { var_dump($e->getMessage()); }
        }
    }
    return "update completed.";
}

//挿入
function insert(string $data)
{
    $pdo = connect_db();
    //データ型は"aut=...=lab|url|tag=val|tag=val...^..."
    $d0 = explode("^", $data);
    foreach($d0 as $data0)
    {
        if($data0 == "") break;
        $d1 = explode("|", $data0);
        $id = "";
        for($i = 0; $i < count($d1); $i++)
        {
            $ed = explode("=", $d1[$i]);
            if($i == 0)
            {
                $aut = $ed[0];
                $cla = $ed[1];
                $clde = $ed[2];
                $ald = $ed[3];
                $sul = $ed[4];
                $eff = $ed[5];
                $ff = $ed[6];
                $voc = $ed[7];
                $jsc = $ed[8];
                $lab = $ed[9];
                //追加
                try
                {
                    $pdo->query("INSERT INTO device (author, class, class_detail, ald, sulfurization, efficiency, ff, voc, jsc, lab)
                                VALUES ('$aut', '$cla', '$clde', '$ald', '$sul', $eff, $ff, $voc, $jsc, $lab)");
                    $id = $pdo->lastInsertId();
                }
                catch(PDOException $e) { var_dump($e->getMessage()); }
            }
            else if($i == 1)
            {
                $url = $ed[0];
                try { $pdo->query("UPDATE device SET url = '$url' WHERE id = $id"); }
                catch(PDOException $e) { var_dump($e->getMessage()); }
            }
            else
            {
                $tag = $ed[0];
                $val = $ed[1];
                //追加
                try { $pdo->query("INSERT INTO extra (device_id, tag, value) VALUES ($id, '$tag', $val)"); }
                catch(PDOException $e) { var_dump($e->getMessage()); }
            }
        }
    }
    return "insert completed.";
}

//削除
function remove(string $data)
{
    $pdo = connect_db();
    //data = id
    try
    {
        $pdo->query("DELETE FROM device WHERE id = $data");
    }
    catch(PDOException $e) { var_dump($e->getMessage()); }
    try
    {
        $pdo->query("DELETE FROM extra WHERE device_id = $data");
    }
    catch(PDOException $e) { var_dump($e->getMessage()); }
    return "remove completed.";
}