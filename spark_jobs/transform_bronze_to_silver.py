import os
from pyspark.sql import SparkSession
from pyspark.sql.functions import col, trim, regexp_replace, initcap

def main():
    os.environ['HADOOP_HOME'] = "C:\\tmp"
    os.makedirs("C:\\tmp", exist_ok=True)

    spark = (
        SparkSession.builder
        .appName("CreditParametersETL")
        .master("local[*]")
        .config("spark.hadoop.fs.file.impl", "org.apache.hadoop.fs.LocalFileSystem")
        .config("spark.hadoop.fs.AbstractFileSystem.file.impl", "org.apache.hadoop.fs.local.LocalFs")
        .getOrCreate()
    )

    bronze_path = os.path.join("data", "bronze", "parameters.csv")
    silver_path = os.path.join("data", "silver")

    print(f"[INFO]  Lendo dados de: {bronze_path}")

    df_bronze = (
        spark.read
        .option("header", "true")
        .option("inferSchema", "true")
        .option("encoding", "utf-8")
        .option("sep", ",")
        .csv(bronze_path)
    )

    print(f"[INFO] Registros lidos: {df_bronze.count()}")
    df_bronze.show(truncate=False)

    df_silver = (
        df_bronze
        .withColumn("Product", trim(initcap(col("Product"))))
        .withColumn("Key", trim(initcap(col("Key"))))
        .withColumn("Value", regexp_replace(col("Value"), "[^0-9.]", "").cast("double"))
    )

    print("[INFO]  Dados após transformação (Silver):")
    df_silver.show(truncate=False)

    os.makedirs(silver_path, exist_ok=True)
    (
        df_silver
        .write
        .mode("overwrite")
        .partitionBy("Product")
        .parquet(silver_path)
    )

    print(f"[SUCCESS]  Silver gerado em: {silver_path}")

    spark.stop()

if __name__ == "__main__":
    main()
