  <?php
$u = $_GET['u'];
$n = $_GET['n'];
$p = $_GET['p'];
$r = $_GET['r'];
$servername = "localhost";
$username = "root";
$password = "Group5";
$dbname = "Creechky";

// Create connection
$con=mysqli_connect($host,$username,$password,$database);

// Check connection
if (mysqli_connect_errno()) {
  echo "Failed to connect to MySQL: " . mysqli_connect_error();
}

$UpdateResult = mysqli_query($con,"UPDATE creechky.user set Name = '{$n}', Phone_number = '{$p}', Restriction_type = '{$r}' where UID = '{$u}'");
if(!$UpdateResult)
{
  echo "Error";
}
else {
  echo "OK";
}
$conn->close();
?>  