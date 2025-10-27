from pyspark.sql import SparkSession

def main():
    spark = (
        SparkSession.builder
        .appName("CompareLayers")
        .master("local[*]")
        .getOrCreate()
    )

    bronze_path = "data/bronze/parameters.csv"
    silver_path = "data/silver"

    df_bronze = spark.read.option("header", "true").csv(bronze_path)

    df_silver = spark.read.parquet(silver_path)

    print("\n===================  Bronze ===================")
    df_bronze.printSchema()
    df_bronze.show(truncate=False)

    print("\n Tipos de dados - Bronze:")
    print(df_bronze.dtypes)

    print("\n===================  Silver ===================")
    df_silver.printSchema()
    df_silver.show(truncate=False)

    print("\n Tipos de dados - Silver:")
    print(df_silver.dtypes)

    spark.stop()

if __name__ == "__main__":
    main()
