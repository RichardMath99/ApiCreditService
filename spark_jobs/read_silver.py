from pyspark.sql import SparkSession

def main():
    spark = (
        SparkSession.builder
        .appName("ReadSilver")
        .master("local[*]")
        .getOrCreate()
    )

    silver_path = "data/silver"

    df_silver = spark.read.parquet(silver_path)

    print("\n Schema do Silver:")
    df_silver.printSchema()

    print("\n Dados da camada Silver:")
    df_silver.show(truncate=False)

    spark.stop()

if __name__ == "__main__":
    main()
