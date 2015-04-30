  <?php
$u = $_GET['u'];
$servername = "localhost";
$username = "root";
$password = "Group5";
$dbname = "Creechky";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
     die("Connection failed: " . $conn->connect_error);
} 
// sql to delete a record
$sql = "DELETE FROM user WHERE UID = '".$u."'";

if ($conn->query($sql) === TRUE) {
    echo "Record deleted successfully";
} else {
    echo "Error deleting record: " . $conn->error;
}
?>  